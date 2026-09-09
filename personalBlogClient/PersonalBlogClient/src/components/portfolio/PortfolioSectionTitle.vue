<template>
  <div class="section-title-wrap">
    <span class="section-eyebrow">{{ eyebrow }}</span>
    <h2 class="section-main-heading">{{ title }}</h2>
    <p v-if="description" class="section-desc">{{ description }}</p>
  </div>
</template>

<script setup lang="ts">
defineProps<{ eyebrow: string; title: string; description?: string }>();
</script>

<style scoped lang="scss">
.section-title-wrap {
  margin-bottom: 24px;
}

.section-eyebrow {
  font-family: var(--font-mono);
  color: var(--accent-primary);
  font-size: 0.72rem;
  font-weight: 700;
  letter-spacing: 0.12em;
  text-transform: uppercase;
  display: inline-block;
  margin-bottom: 6px;
}

.section-main-heading {
  font-family: var(--font-headline);
  color: var(--text-primary);
  font-size: clamp(1.85rem, 3.8vw, 2.6rem);
  font-weight: 800;
  letter-spacing: -0.03em;
  line-height: 1.12;
  margin: 0 0 10px;
}

.section-desc {
  font-family: var(--font-body);
  color: var(--text-secondary);
  font-size: 0.95rem;
  line-height: 1.6;
  margin: 0;
  max-width: 620px;
}
</style>
