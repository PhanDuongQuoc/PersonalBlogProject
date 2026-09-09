<template>
  <section class="about-stats-section">
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-icon-wrap">
          <q-icon name="work_history" size="22px" />
        </div>
        <div class="stat-info">
          <strong>{{ stats.yearsOfExperience }}+</strong>
          <span>{{ text.yearsExpLabel }}</span>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon-wrap">
          <q-icon name="article" size="22px" />
        </div>
        <div class="stat-info">
          <strong>{{ stats.publishedPostCount }}</strong>
          <span>{{ text.publishedPostsLabel }}</span>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon-wrap">
          <q-icon name="folder_open" size="22px" />
        </div>
        <div class="stat-info">
          <strong>{{ stats.categoryCount }}</strong>
          <span>{{ text.topicsExploredLabel }}</span>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon-wrap">
          <q-icon name="visibility" size="22px" />
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
  border-bottom: 1px solid var(--border-hairline);
  padding: 48px 0;
}

.stats-grid {
  display: grid;
  gap: 20px;
  grid-template-columns: repeat(4, minmax(0, 1fr));
}

.stat-card {
  align-items: center;
  background: var(--bg-surface-low);
  border: 1px solid var(--border-hairline);
  border-radius: var(--radius-lg);
  display: flex;
  gap: 16px;
  padding: 20px 22px;
  box-shadow: var(--shadow-card);
  transition: transform 0.2s ease, border-color 0.2s ease;

  &:hover {
    border-color: rgba(16, 185, 129, 0.35);
    transform: translateY(-2px);
  }
}

.stat-icon-wrap {
  align-items: center;
  background: var(--accent-primary-container);
  border-radius: var(--radius-md);
  color: var(--accent-primary);
  display: flex;
  height: 44px;
  justify-content: center;
  width: 44px;
  flex-shrink: 0;
}

.stat-info {
  display: flex;
  flex-direction: column;

  strong {
    font-family: var(--font-mono);
    color: var(--accent-primary);
    font-size: 1.5rem;
    font-weight: 800;
    line-height: 1.1;
  }

  span {
    font-family: var(--font-mono);
    color: var(--text-muted);
    font-size: 0.72rem;
    font-weight: 600;
    text-transform: uppercase;
    letter-spacing: 0.04em;
    margin-top: 4px;
  }
}

@media (max-width: 900px) {
  .stats-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}

@media (max-width: 600px) {
  .stats-grid {
    grid-template-columns: 1fr;
  }
}
</style>
