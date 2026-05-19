<script setup>
import { ref } from 'vue'

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000'
const placeId = ref('')
const reviews = ref([])
const loading = ref(false)
const error = ref('')

const fetchData = async () => {
  const trimmedPlaceId = placeId.value.trim()

  if (!trimmedPlaceId) {
    error.value = 'Vui lòng nhập Place ID trước khi lấy review'
    reviews.value = []
    return
  }

  loading.value = true
  error.value = ''

  try {
    const response = await fetch(`${apiBaseUrl}/api/data?placeId=${encodeURIComponent(trimmedPlaceId)}`)

    if (!response.ok) {
      throw new Error('Không thể tải dữ liệu')
    }

    reviews.value = await response.json()
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Không thể tải dữ liệu'
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
</script>

<template>
  <main class="mx-auto max-w-5xl p-6">
    <h1 class="mb-4 text-2xl font-bold text-slate-900">UCOrm - Dashboard Reviews</h1>

    <section class="mb-6 rounded-xl bg-white p-4 shadow">
      <label class="mb-2 block text-sm font-medium text-slate-700" for="placeId">Place ID</label>
      <div class="flex flex-col gap-3 sm:flex-row">
        <input
          id="placeId"
          v-model="placeId"
          class="w-full rounded border border-slate-300 px-3 py-2 outline-none focus:border-blue-500"
          placeholder="Ví dụ: vinpearl-luxury-landmark-81"
          @keyup.enter="fetchData"
        />
        <button
          class="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white disabled:opacity-50"
          @click="fetchData"
          :disabled="loading"
        >
          Lấy Review
        </button>
      </div>
      <p class="mt-2 text-sm text-slate-500">Nhập Place ID rồi mới tải danh sách review tương ứng. ở đây vì lấy từ mock data chứ ko gọi place api của gg nên hãy nhập "vinpearl-luxury-landmark-81"</p>
    </section>

    <p v-if="loading" class="mb-4 text-slate-600">Đang tải dữ liệu...</p>
    <p v-if="error" class="mb-4 rounded bg-red-100 p-3 text-red-700">{{ error }}</p>

    <p v-if="!loading && reviews.length === 0 && !error" class="mb-4 rounded bg-slate-100 p-3 text-slate-600">
      Chưa có dữ liệu. Hãy nhập Place ID và bấm Lấy Review.
    </p>

    <div class="grid gap-4">
      <article v-for="review in reviews" :key="review.reviewId" class="rounded bg-white p-4 shadow">
        <div class="mb-2 flex items-center justify-between">
          <strong>{{ review.authorName }}</strong>
          <span
            class="rounded px-2 py-1 text-xs"
            :class="review.status === 'Resolved' ? 'bg-green-100 text-green-700' : 'bg-yellow-100 text-yellow-700'"
          >
            {{ review.status }}
          </span>
        </div>

        <p class="mb-2 text-sm text-slate-700">⭐ {{ review.rating }} - {{ review.reviewText }}</p>

        <div class="flex gap-2">
          <button
            class="rounded bg-blue-600 px-3 py-1 text-sm text-white"
            @click="callAi(review)"
            :disabled="loading || review.status == 'Resolved'"
          >
            Gợi ý AI
          </button>
          <button
            v-if="review.aiSuggestions?.standard"
            class="rounded bg-emerald-600 px-3 py-1 text-sm text-white"
            @click="resolve(review, review.aiSuggestions.standard)"
            :disabled="loading || review.status == 'Resolved'"
          >
            Duyệt Standard
          </button>
          <button
            v-if="review.aiSuggestions?.friendly"
            class="rounded bg-emerald-600 px-3 py-1 text-sm text-white"
            @click="resolve(review, review.aiSuggestions.friendly)"
            :disabled="loading || review.status == 'Resolved'"
          >
            Duyệt Friendly
          </button>
          <button
            v-if="review.aiSuggestions?.apology"
            class="rounded bg-emerald-600 px-3 py-1 text-sm text-white"
            @click="resolve(review, review.aiSuggestions.apology)"
            :disabled="loading || review.status == 'Resolved'"
          >
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
