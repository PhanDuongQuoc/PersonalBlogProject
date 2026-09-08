<template>
  <div class="technical-index-table">
    <article
      v-for="(category, idx) in categories"
      :key="category.slug"
      class="index-row-item"
    >
      <!-- Index Number -->
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
    </article>
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
  margin-top: 32px;
  border-top: 1px solid rgba(255, 255, 255, 0.08);
}

.index-row-item {
  display: grid;
  grid-template-columns: 48px 1fr auto 32px;
  align-items: center;
  gap: 20px;
  padding: 20px 12px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.06);
  transition: all 0.2s ease;

  &:hover {
    background: rgba(255, 255, 255, 0.02);
    padding-left: 20px;

    .index-num {
      color: #46e0af;
    }

    .index-topic-name {
      color: #46e0af;
    }

    .row-arrow-icon {
      transform: translateX(4px);
      color: #46e0af;
    }
  }
}

.index-num {
  font-family: monospace;
  font-size: 0.85rem;
  font-weight: 700;
  color: #64748b;
  transition: color 0.2s ease;
}

.index-main-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.index-topic-name {
  font-size: 1.15rem;
  font-weight: 700;
  color: #f1f5f9;
  line-height: 1.2;
  margin: 0;
  transition: color 0.2s ease;
}

.index-topic-slug {
  font-family: monospace;
  font-size: 0.75rem;
  color: #64748b;
}

.index-stat-col {
  display: flex;
  align-items: center;
}

.count-pill {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-size: 0.78rem;
  font-weight: 600;
  color: #cbd5e1;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.08);
  padding: 4px 10px;
  border-radius: 4px;
}

.index-arrow-col {
  display: flex;
  align-items: center;
  justify-content: flex-end;
}

.row-arrow-icon {
  color: #475569;
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
  .count-pill {
    font-size: 0.72rem;
    padding: 3px 8px;
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

<!-- Global Light Mode overrides -->
<style lang="scss">
body.portfolio-light {
  .technical-index-table {
    border-color: #e2e8f0 !important;
  }

  .index-row-item {
    border-color: #f1f5f9 !important;

    &:hover {
      background: #f8fafc !important;

      .index-num,
      .index-topic-name,
      .row-arrow-icon {
        color: #0f9f74 !important;
      }
    }
  }

  .index-num {
    color: #94a3b8 !important;
  }

  .index-topic-name {
    color: #0f172a !important;
  }

  .index-topic-slug {
    color: #64748b !important;
  }

  .count-pill {
    background: #f1f5f9 !important;
    border-color: #e2e8f0 !important;
    color: #475569 !important;
  }

  .row-arrow-icon {
    color: #94a3b8 !important;
  }
}
</style>
