<template>
  <div v-if="posts.length" class="article-grid">
    <article v-for="(post, index) in posts" :key="post.id" class="article-card"><div class="post-visual" :class="`visual-${index % 3}`"><img v-if="post.thumbnailUrl" :src="post.thumbnailUrl" :alt="post.title" /><q-icon v-else name="code" /><span>{{ post.category }}</span></div><div class="article-content"><p>{{ post.category }} <span>{{ formatDate(post.publishedAt) }}</span></p><h3>{{ post.title }}</h3><div class="article-bottom"><span>{{ post.excerpt || text.readLatest }}</span><q-icon name="north_east" /></div></div></article>
  </div>
  <div v-else class="empty-state"><q-icon name="edit_note" size="30px" /><p>{{ text.articlesPreparing }}</p></div>
</template>

<script setup lang="ts">
import type { PublicPost } from '@/types/public-landing';
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';
defineProps<{ posts: PublicPost[] }>();
const { locale, text } = usePortfolioLocale();
const formatDate = (date: string | null) => date ? new Intl.DateTimeFormat(locale.value === 'vi' ? 'vi-VN' : 'en', { month: 'short', year: 'numeric' }).format(new Date(date)) : text.value.recent;
</script>

<style scoped lang="scss">
.article-grid { display: grid; gap: 22px; grid-template-columns: repeat(3, minmax(0, 1fr)); margin-top: 30px; }.article-card { background: #151f37; border: 1px solid #26324e; border-radius: 2px; overflow: hidden; }.post-visual { align-items: center; background: linear-gradient(135deg, #0e172b, #1c2d49); display: flex; height: 170px; justify-content: center; overflow: hidden; position: relative; }.post-visual img { height: 100%; object-fit: cover; width: 100%; }.post-visual .q-icon { color: #35d6ff; font-size: 3rem; }.post-visual span { bottom: 15px; color: #d4e5ff; font-size: .65rem; font-weight: 800; letter-spacing: .12em; position: absolute; text-transform: uppercase; }.visual-1 { background: linear-gradient(135deg, #192745, #0e594f); }.visual-2 { background: linear-gradient(135deg, #252343, #34466d); }.article-content { padding: 21px; }.article-card p { color: #46e0af; font-size: .68rem; font-weight: 800; letter-spacing: .1em; margin: 0; text-transform: uppercase; }.article-card p span { color: #91a9be; font-weight: 500; margin-left: 7px; }.article-card h3 { color: #f0f4ff; font-size: 1.3rem; font-weight: 800; letter-spacing: -.035em; line-height: 1.16; margin: 17px 0; }.article-bottom { align-items: end; color: #a9c1d3; display: flex; font-size: .8rem; gap: 16px; justify-content: space-between; line-height: 1.55; }.article-bottom .q-icon { color: #46e0af; flex: 0 0 auto; }.empty-state { align-items: center; background: #151f37; border: 1px dashed #355071; border-radius: 2px; color: #a9c1d3; display: flex; gap: 14px; padding: 28px; }.empty-state p { margin: 0; } @media (max-width: 800px) { .article-grid { grid-template-columns: 1fr; } }
</style>
