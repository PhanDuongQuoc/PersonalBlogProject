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
      <q-spinner color="teal-5" size="36px" />
      <p>{{ text.loading }}</p>
    </div>

    <!-- 5. Footer -->
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
  max-width: 1080px;
  padding: 0 28px;
  box-sizing: border-box;
}

.content-section {
  padding: 72px 0;
  border-bottom: 1px solid rgba(255, 255, 255, 0.06);
}

@media (max-width: 860px) {
  .portfolio-content {
    padding: 0 20px;
  }
  .content-section {
    padding: 52px 0;
  }
}

@media (max-width: 500px) {
  .portfolio-content {
    padding: 0 16px;
  }
  .content-section {
    padding: 38px 0;
  }
}

/* Narrative / About Section */
.narrative-grid {
  display: grid;
  grid-template-columns: 1.15fr 0.85fr;
  gap: 48px;
  align-items: center;
}

.narrative-left {
  display: flex;
  flex-direction: column;
}

.narrative-body-text {
  font-size: 1rem;
  line-height: 1.7;
  color: #94a3b8;
  margin: 18px 0 24px;
}

.narrative-action {
  display: flex;
}

.view-dossier-btn {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 10px 20px;
  border-radius: 6px;
  background: rgba(70, 224, 175, 0.12);
  color: #46e0af;
  border: 1px solid rgba(70, 224, 175, 0.25);
  font-size: 0.875rem;
  font-weight: 700;
  text-decoration: none;
  transition: all 0.2s ease;

  &:hover {
    background: #46e0af;
    color: #081126;
  }
}

/* Pillars Stack */
.pillars-stack {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.pillar-item {
  display: grid;
  grid-template-columns: 36px 1fr;
  gap: 16px;
  padding: 18px;
  background: rgba(15, 23, 42, 0.4);
  border: 1px solid rgba(255, 255, 255, 0.06);
  border-radius: 8px;
  transition: all 0.2s ease;

  &:hover {
    border-color: rgba(70, 224, 175, 0.25);
    background: rgba(15, 23, 42, 0.6);
  }
}

.pillar-num {
  font-family: monospace;
  font-size: 0.85rem;
  font-weight: 700;
  color: #46e0af;
}

.pillar-detail {
  h4 {
    font-size: 0.95rem;
    font-weight: 700;
    color: #f1f5f9;
    margin: 0 0 6px;
  }

  p {
    font-size: 0.82rem;
    line-height: 1.5;
    color: #94a3b8;
    margin: 0;
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
  color: #94a3b8;

  &.error-state {
    color: #f87171;
  }
}

@media (max-width: 860px) {
  .narrative-grid {
    grid-template-columns: 1fr;
    gap: 36px;
  }
}
</style>

<!-- Global Light Mode overrides -->
<style lang="scss">
body.portfolio-light {
  .content-section {
    border-color: #e2e8f0 !important;
  }

  .narrative-body-text {
    color: #475569 !important;
  }

  .view-dossier-btn {
    background: rgba(15, 159, 116, 0.1) !important;
    color: #0f9f74 !important;
    border-color: rgba(15, 159, 116, 0.25) !important;

    &:hover {
      background: #0f9f74 !important;
      color: #ffffff !important;
    }
  }

  .pillar-item {
    background: #ffffff !important;
    border-color: #e2e8f0 !important;

    &:hover {
      border-color: rgba(15, 159, 116, 0.3) !important;
      background: #f8fafc !important;
    }
  }

  .pillar-num {
    color: #0f9f74 !important;
  }

  .pillar-detail {
    h4 {
      color: #0f172a !important;
    }
    p {
      color: #475569 !important;
    }
  }
}
</style>
