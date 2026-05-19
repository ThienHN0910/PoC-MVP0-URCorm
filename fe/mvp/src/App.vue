<script setup>
import { ref, onMounted } from 'vue'

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000'
const reviews = ref([])
const loading = ref(false)
const error = ref('')

const fetchData = async () => {
  loading.value = true
  error.value = ''
  try {
    const response = await fetch(`${apiBaseUrl}/api/data`)
    if (!response.ok) {
      throw new Error('Không thể tải dữ liệu')
    }

    reviews.value = await response.json()
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}

const callAi = async (review) => {
  error.value = ''
  const response = await fetch(`${apiBaseUrl}/api/callAI`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      reviewId: review.reviewId,
      reviewText: review.reviewText,
      rating: review.rating
    })
  })

  if (!response.ok) {
    error.value = 'Gọi AI thất bại'
    return
  }

  review.aiSuggestions = await response.json()
}

const resolve = async (review, reply) => {
  error.value = ''
  const response = await fetch(`${apiBaseUrl}/api/data`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ reviewId: review.reviewId, selectedReply: reply })
  })

  if (!response.ok) {
    error.value = 'Cập nhật phản hồi thất bại'
    return
  }

  review.selectedReply = reply
  review.status = 'Resolved'
}

onMounted(fetchData)
</script>

<template>
  <main class="mx-auto max-w-5xl p-6">
    <h1 class="mb-4 text-2xl font-bold text-slate-900">UCOrm - Dashboard Reviews</h1>
    <p v-if="loading" class="mb-4 text-slate-600">Đang tải dữ liệu...</p>
    <p v-if="error" class="mb-4 rounded bg-red-100 p-3 text-red-700">{{ error }}</p>

    <div class="grid gap-4">
      <article v-for="review in reviews" :key="review.reviewId" class="rounded bg-white p-4 shadow">
        <div class="mb-2 flex items-center justify-between">
          <strong>{{ review.authorName }}</strong>
          <span class="rounded px-2 py-1 text-xs" :class="review.status === 'Resolved' ? 'bg-green-100 text-green-700' : 'bg-yellow-100 text-yellow-700'">{{ review.status }}</span>
        </div>

        <p class="mb-2 text-sm text-slate-700">⭐ {{ review.rating }} - {{ review.reviewText }}</p>

        <div class="flex gap-2">
          <button class="rounded bg-blue-600 px-3 py-1 text-sm text-white" @click="callAi(review)" :disabled="loading || review.status == 'Resolved'">Gợi ý AI</button>
          <button
            v-if="review.aiSuggestions?.standard"
            class="rounded bg-emerald-600 px-3 py-1 text-sm text-white"
            @click="resolve(review, review.aiSuggestions.standard)"
            :disabled="loading || review.status == 'Resolved'">
            Duyệt Standard
          </button>
          <button
            v-if="review.aiSuggestions?.friendly"
            class="rounded bg-emerald-600 px-3 py-1 text-sm text-white"
            @click="resolve(review, review.aiSuggestions.friendly)"
            :disabled="loading || review.status == 'Resolved'">
            Duyệt Friendly
          </button>
          <button
            v-if="review.aiSuggestions?.apology"
            class="rounded bg-emerald-600 px-3 py-1 text-sm text-white"
            @click="resolve(review, review.aiSuggestions.apology)"
            :disabled="loading || review.status == 'Resolved'">
            Duyệt Apology
          </button>
        </div>

        <div v-if="review.aiSuggestions" class="mt-3 space-y-1 rounded bg-slate-50 p-3 text-sm">
          <p><b>Standard:</b> {{ review.aiSuggestions.standard }}</p>
          <p><b>Friendly:</b> {{ review.aiSuggestions.friendly }}</p>
          <p><b>Apology:</b> {{ review.aiSuggestions.apology }}</p>
        </div>

        <p v-if="review.selectedReply" class="mt-3 rounded bg-green-50 p-3 text-sm text-green-800">
          <b>Selected Reply:</b> {{ review.selectedReply }}
        </p>
      </article>
    </div>
  </main>
</template>
