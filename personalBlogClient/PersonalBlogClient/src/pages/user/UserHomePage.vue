<template>
  <q-page class="portfolio-page">
    <main v-if="landing" class="portfolio-content">
      <!-- 1. Hero Section -->
      <portfolio-hero :profile="landing.profile" :stats="landing.stats" />

      <!-- 2. Technical Topics / Categories Index -->
      <section id="topics" class="content-section topics-section">
        <portfolio-section-title
          :eyebrow="text.browseByCategory"
          :title="text.topics"
          :description="text.topicsDescription"
        />
        <portfolio-categories :categories="landing.categories" />
      </section>

      <!-- 3. Featured Writing & Engineering Notes -->
      <section id="posts" class="content-section writing-section">
        <portfolio-section-title
          :eyebrow="text.fromTheBlog"
          :title="text.featuredPosts"
          :description="text.readLatest"
        />
        <portfolio-articles :posts="landing.featuredPosts" />
      </section>
    </main>

    <!-- Loading / Error States -->
    <div v-else-if="errorMessage" class="load-state error-state">
      <q-icon name="error_outline" size="44px" color="negative" />
      <p>{{ errorMessage }}</p>
    </div>
    
    <div v-else class="load-state">
      <q-spinner color="teal-4" size="36px" />
      <p>{{ text.loading }}</p>
    </div>

    <!-- 4. Footer -->
    <portfolio-footer :email="landing?.profile.email ?? null" />
  </q-page>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';
import api from '@/boot/ApiGateway/axios';
import PortfolioArticles from '@/components/portfolio/PortfolioArticles.vue';
import PortfolioCategories from '@/components/portfolio/PortfolioCategories.vue';
import PortfolioFooter from '@/components/portfolio/PortfolioFooter.vue';
import PortfolioHero from '@/components/portfolio/PortfolioHero.vue';
import PortfolioSectionTitle from '@/components/portfolio/PortfolioSectionTitle.vue';
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';
import type { PublicLandingResponse } from '@/types/public-landing';

const landing = ref<PublicLandingResponse | null>(null);
const errorMessage = ref('');
const route = useRoute();
const { text } = usePortfolioLocale();

onMounted(async () => {
  try {
    const username = typeof route.query.username === 'string' ? route.query.username : undefined;
    const response = await api.get<PublicLandingResponse>('/public/landing', { params: { username } });
    landing.value = response.data;
  } catch {
    errorMessage.value = text.value.loadFailed;
  }
});
</script>

<style scoped lang="scss">
.portfolio-page {
  background: transparent;
  min-height: 100vh;
  overflow-x: hidden;
}

.portfolio-content {
  margin: 0 auto;
  max-width: 1140px;
  padding: 0 32px;
  box-sizing: border-box;
}

.content-section {
  padding: 80px 0;
  border-bottom: 1px solid var(--border-hairline);
}

@media (max-width: 860px) {
  .portfolio-content {
    padding: 0 20px;
  }
  .content-section {
    padding: 56px 0;
  }
}

@media (max-width: 500px) {
  .portfolio-content {
    padding: 0 16px;
  }
  .content-section {
    padding: 40px 0;
  }
}

/* Load state */
.load-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 120px 20px;
  gap: 16px;
  color: var(--text-secondary);

  &.error-state {
    color: #f87171;
  }
}
</style>
