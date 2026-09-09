<template>
  <div class="technical-index-table">
    <router-link
      v-for="(category, idx) in categories"
      :key="category.slug"
      :to="`/topics/${category.slug}`"
      class="index-row-item"
    >
      <!-- Index Number (01, 02, etc.) -->
      <span class="index-num">0{{ idx + 1 }}</span>

      <!-- Topic Title & Slug -->
      <div class="index-main-info">
        <h3 class="index-topic-name">
          {{ category.name }}
        </h3>
        <span class="index-topic-slug">/topics/{{ category.slug }}</span>
      </div>

      <!-- Article Count Metric -->
      <div class="index-stat-col">
        <span class="count-pill">
          <q-icon name="article" size="13px" />
          {{ category.postCount }} {{ text.publishedPosts }}
        </span>
      </div>

      <!-- Arrow Indicator -->
      <div class="index-arrow-col">
        <q-icon name="arrow_forward" size="16px" class="row-arrow-icon" />
      </div>
    </router-link>

    <!-- View All Topics Link -->
    <div class="topics-more-action">
      <router-link to="/topics" class="view-all-topics-btn">
        <span>{{ text.viewAllTopics }}</span>
        <q-icon name="arrow_forward" size="16px" class="action-arrow" />
      </router-link>
    </div>
  </div>
</template>

<script setup lang="ts">
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';

defineProps<{
  categories: {
    name: string;
    slug: string;
    postCount: number;
  }[];
}>();

const { text } = usePortfolioLocale();
</script>

<style scoped lang="scss">
.technical-index-table {
  display: flex;
  flex-direction: column;
  margin-top: 28px;
  border-top: 1px solid var(--border-hairline);
}

.index-row-item {
  display: grid;
  grid-template-columns: 48px 1fr auto 32px;
  align-items: center;
  gap: 20px;
  padding: 20px 12px;
  border-bottom: 1px solid var(--border-hairline);
  text-decoration: none;
  cursor: pointer;
  transition: all 0.2s ease;

  &:hover {
    background: var(--accent-primary-container);
    padding-left: 20px;

    .index-num {
      color: var(--accent-primary);
    }

    .index-topic-name {
      color: var(--accent-primary);
    }

    .row-arrow-icon {
      transform: translateX(4px);
      color: var(--accent-primary);
    }
  }
}

.topics-more-action {
  display: flex;
  justify-content: flex-end;
  padding: 20px 8px 0;
}

.view-all-topics-btn {
  font-family: var(--font-headline);
  display: inline-flex;
  align-items: center;
  gap: 8px;
  color: var(--accent-primary);
  text-decoration: none;
  font-size: 0.88rem;
  font-weight: 700;
  transition: all 0.2s ease;

  .action-arrow {
    transition: transform 0.2s ease;
  }

  &:hover {
    .action-arrow {
      transform: translateX(4px);
    }
  }
}

.index-num {
  font-family: var(--font-mono);
  font-size: 0.85rem;
  font-weight: 700;
  color: var(--text-muted);
  transition: color 0.2s ease;
}

.index-main-info {
  display: flex;
  flex-direction: column;
  gap: 3px;
}

.index-topic-name {
  font-family: var(--font-headline);
  font-size: 1.15rem;
  font-weight: 700;
  color: var(--text-primary);
  line-height: 1.25;
  margin: 0;
  letter-spacing: -0.015em;
  transition: color 0.2s ease;
}

.index-topic-slug {
  font-family: var(--font-mono);
  font-size: 0.74rem;
  color: var(--text-muted);
}

.index-stat-col {
  display: flex;
  align-items: center;
}

.count-pill {
  font-family: var(--font-mono);
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-size: 0.74rem;
  font-weight: 600;
  color: var(--text-secondary);
  background: var(--bg-surface-high);
  border: 1px solid var(--border-subtle);
  padding: 4px 10px;
  border-radius: var(--radius-pill);
}

.index-arrow-col {
  display: flex;
  align-items: center;
  justify-content: flex-end;
}

.row-arrow-icon {
  color: var(--text-muted);
  transition: all 0.2s ease;
}

@media (max-width: 640px) {
  .index-row-item {
    grid-template-columns: 32px 1fr auto;
    gap: 12px;
    padding: 16px 8px;
  }
  .index-arrow-col {
    display: none;
  }
  .index-topic-name {
    font-size: 1.02rem;
  }
}

@media (max-width: 420px) {
  .index-row-item {
    grid-template-columns: 28px 1fr;
    gap: 10px;
    padding: 14px 4px;
  }
  .index-stat-col {
    grid-column: 2 / -1;
    margin-top: 4px;
  }
}
</style>
