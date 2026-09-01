<template>
  <div class="category-list">
    <article v-for="category in categories" :key="category.slug" class="category-card">
      <q-icon name="folder_open" size="22px" />
      <div><h3>{{ category.name }}</h3><p>{{ category.postCount }} {{ text.publishedPosts }}</p></div>
      <q-icon name="arrow_forward" />
    </article>
  </div>
</template>

<script setup lang="ts">
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';
defineProps<{ categories: { name: string; slug: string; postCount: number }[] }>();
const { text } = usePortfolioLocale();
</script>

<style scoped lang="scss">
.category-list { display: grid; gap: 16px; grid-template-columns: repeat(3, minmax(0, 1fr)); margin-top: 28px; }.category-card { align-items: center; background: #151f37; border: 1px solid #26324e; border-radius: 2px; display: grid; gap: 13px; grid-template-columns: auto 1fr auto; min-height: 116px; padding: 20px; }.category-card > .q-icon:first-child { color: #35d6ff; }.category-card > .q-icon:last-child { color: #46e0af; }.category-card h3 { color: #dce8ff; font-size: .9rem; margin: 0; }.category-card p { color: #9db4ca; font-size: .75rem; margin: 5px 0 0; } @media (max-width: 700px) { .category-list { grid-template-columns: 1fr; } }
</style>
