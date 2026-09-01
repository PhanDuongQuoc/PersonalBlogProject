<template>
  <div class="section-title">
    <span>{{ eyebrow }}</span>
    <h2>{{ title }}</h2>
    <p v-if="description">{{ description }}</p>
  </div>
</template>

<script setup lang="ts">
defineProps<{ eyebrow: string; title: string; description?: string }>();
</script>

<style scoped lang="scss">
.section-title span { color: #46e0af; font-size: .72rem; font-weight: 800; letter-spacing: .13em; text-transform: uppercase; }
h2 { color: #f1f4ff; font-size: clamp(2rem, 4vw, 3rem); font-weight: 900; letter-spacing: -.06em; line-height: 1.05; margin: 10px 0; }
p { color: #a9c1d3; font-size: .9rem; line-height: 1.65; margin: 0; max-width: 570px; }
</style>
