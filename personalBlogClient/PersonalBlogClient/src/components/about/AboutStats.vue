<template>
  <section class="about-stats-section">
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-icon-wrap">
          <q-icon name="work_history" size="24px" />
        </div>
        <div class="stat-info">
          <strong>{{ stats.yearsOfExperience }}+</strong>
          <span>{{ text.yearsExpLabel }}</span>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon-wrap">
          <q-icon name="article" size="24px" />
        </div>
        <div class="stat-info">
          <strong>{{ stats.publishedPostCount }}</strong>
          <span>{{ text.publishedPostsLabel }}</span>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon-wrap">
          <q-icon name="folder_open" size="24px" />
        </div>
        <div class="stat-info">
          <strong>{{ stats.categoryCount }}</strong>
          <span>{{ text.topicsExploredLabel }}</span>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon-wrap">
          <q-icon name="visibility" size="24px" />
        </div>
        <div class="stat-info">
          <strong>{{ stats.totalViewCount }}</strong>
          <span>{{ text.totalViewsLabel }}</span>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';
import type { PublicAboutStats } from '@/types/public-about';

defineProps<{ stats: PublicAboutStats }>();
const { text } = usePortfolioLocale();
</script>

<style scoped lang="scss">
.about-stats-section {
  border-top: 1px solid #202b4a;
  padding: 60px 78px;
}

.stats-grid {
  display: grid;
  gap: 20px;
  grid-template-columns: repeat(4, minmax(0, 1fr));
}

.stat-card {
  align-items: center;
  background: #151f37;
  border: 1px solid #26324e;
  border-radius: 4px;
  display: flex;
  gap: 16px;
  padding: 22px 24px;
  transition: transform 0.2s ease, border-color 0.2s ease;
}

.stat-card:hover {
  border-color: #38517c;
  transform: translateY(-2px);
}

.stat-icon-wrap {
  align-items: center;
  background: rgba(53, 214, 255, 0.1);
  border-radius: 8px;
  color: #35d6ff;
  display: flex;
  height: 48px;
  justify-content: center;
  width: 48px;
}

.stat-info {
  display: flex;
  flex-direction: column;
}

.stat-info strong {
  color: #46e0af;
  font-size: 1.65rem;
  font-weight: 900;
  line-height: 1.1;
}

.stat-info span {
  color: #a0b6cc;
  font-size: 0.75rem;
  font-weight: 700;
  margin-top: 4px;
}

@media (max-width: 900px) {
  .stats-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}

@media (max-width: 760px) {
  .about-stats-section {
    padding: 48px 24px;
  }

  .stats-grid {
    grid-template-columns: 1fr;
  }
}
</style>
