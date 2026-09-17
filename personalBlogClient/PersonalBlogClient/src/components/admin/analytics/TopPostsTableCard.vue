<template>
  <div class="top-posts-table-card">
    <header class="card-header">
      <div class="header-left">
        <div class="header-icon-box">
          <q-icon name="fa-solid fa-trophy" size="14px" />
        </div>
        <h3 class="card-title">Top Bài Viết Đọc Nhiều Nhất</h3>
      </div>

      <router-link to="/admin/posts" class="btn-view-all">
        <span>Xem tất cả bài viết</span>
        <q-icon name="fa-solid fa-arrow-right" size="11px" />
      </router-link>
    </header>

    <div v-if="loading" class="table-loading">
      <q-spinner-tail color="pink-7" size="28px" />
      <span>Đang tải danh sách...</span>
    </div>

    <div v-else-if="posts && posts.length > 0" class="top-posts-list">
      <div
        v-for="(post, idx) in posts"
        :key="post.id"
        class="top-post-item"
      >
        <!-- Rank Number Badge -->
        <div class="rank-badge" :class="`rank-${idx + 1}`">
          <span v-if="idx < 3">#{{ idx + 1 }}</span>
          <span v-else>{{ idx + 1 }}</span>
        </div>

        <!-- Post Title & Meta -->
        <div class="post-info">
          <router-link :to="`/admin/posts`" class="post-title" :title="post.title">
            {{ post.title }}
          </router-link>
          <div class="post-sub-meta">
            <span class="category-pill">{{ post.categoryName }}</span>
            <span class="meta-dot">·</span>
            <span class="comment-count-text">
              <q-icon name="fa-solid fa-comment" size="10px" class="q-mr-xs text-grey-6" />
              {{ post.commentCount }} bình luận
            </span>
          </div>
        </div>

        <!-- Views Metric -->
        <div class="views-metric-box">
          <div class="views-num">{{ post.viewCount.toLocaleString() }}</div>
          <div class="views-label">lượt xem</div>
        </div>
      </div>
    </div>

    <div v-else class="empty-posts">
      <q-icon name="fa-solid fa-newspaper" size="24px" class="text-grey-5 q-mb-xs" />
      <div>Chưa có dữ liệu bài viết</div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import type { TopPerformingPost } from "@/types/admin-analytics";
import { adminAnalyticsService } from "@/services/admin-analytics.service";

const posts = ref<TopPerformingPost[]>([]);
const loading = ref(false);

async function fetchTopPosts() {
  try {
    loading.value = true;
    posts.value = await adminAnalyticsService.getTopPosts(5);
  } catch (err) {
    console.error("Failed to fetch top posts:", err);
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  fetchTopPosts();
});
</script>

<style scoped lang="scss">
.top-posts-table-card {
  background: var(--bg-surface, #0b1326);
  border: 1px solid var(--border-hairline, rgba(248, 250, 252, 0.08));
  border-radius: 16px;
  box-shadow: var(--shadow-card, 0 4px 20px rgba(0, 0, 0, 0.3));
  padding: 20px 22px;
  display: flex;
  flex-direction: column;
  height: 100%;
  box-sizing: border-box;
  transition: all 0.25s ease;

  &:hover {
    box-shadow: var(--shadow-card-hover, 0 8px 26px rgba(0, 0, 0, 0.4));
    border-color: var(--border-subtle, rgba(248, 250, 252, 0.15));
  }
}

.card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  margin-bottom: 18px;
  flex-wrap: wrap;

  .header-left {
    display: flex;
    align-items: center;
    gap: 10px;

    .header-icon-box {
      width: 30px;
      height: 30px;
      border-radius: 8px;
      background: rgba(245, 158, 11, 0.15);
      color: #f59e0b;
      display: flex;
      align-items: center;
      justify-content: center;
      border: 1px solid rgba(245, 158, 11, 0.25);
    }

    .card-title {
      font-family: var(--font-headline, sans-serif);
      font-size: 15.5px;
      font-weight: 700;
      color: var(--text-primary, #dae2fd);
      margin: 0;
    }
  }

  .btn-view-all {
    font-family: var(--font-headline, sans-serif);
    font-size: 12px;
    font-weight: 700;
    color: var(--accent-primary, #df266a);
    text-decoration: none;
    display: inline-flex;
    align-items: center;
    gap: 6px;
    transition: all 0.2s ease;

    &:hover {
      color: var(--accent-primary-hover, #f43f7e);
      transform: translateX(2px);
    }
  }
}

.top-posts-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
  flex: 1;
}

.top-post-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 10px 12px;
  border-radius: 10px;
  background: var(--bg-surface-container, #131b2e);
  border: 1px solid var(--border-hairline, rgba(248, 250, 252, 0.08));
  transition: all 0.2s ease;

  &:hover {
    background: var(--accent-primary-container, rgba(223, 38, 106, 0.12));
    border-color: var(--accent-primary, #df266a);
  }

  .rank-badge {
    width: 28px;
    height: 28px;
    border-radius: 7px;
    display: flex;
    align-items: center;
    justify-content: center;
    font-family: var(--font-mono, monospace);
    font-size: 12px;
    font-weight: 800;
    flex-shrink: 0;
    background: rgba(255, 255, 255, 0.08);
    color: var(--text-primary, #dae2fd);

    &.rank-1 { background: rgba(245, 158, 11, 0.2); color: #fbbf24; border: 1px solid rgba(245, 158, 11, 0.35); }
    &.rank-2 { background: rgba(148, 163, 184, 0.2); color: #cbd5e1; border: 1px solid rgba(148, 163, 184, 0.35); }
    &.rank-3 { background: rgba(234, 88, 12, 0.2); color: #fb923c; border: 1px solid rgba(234, 88, 12, 0.35); }
  }

  .post-info {
    flex: 1;
    min-width: 0;

    .post-title {
      font-family: var(--font-headline, sans-serif);
      font-size: 13.5px;
      font-weight: 700;
      color: var(--text-primary, #dae2fd);
      text-decoration: none;
      display: block;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
      margin-bottom: 3px;

      &:hover {
        color: var(--accent-primary, #df266a);
      }
    }

    .post-sub-meta {
      display: flex;
      align-items: center;
      gap: 6px;
      font-size: 11.5px;

      .category-pill {
        font-family: var(--font-mono, monospace);
        font-size: 10.5px;
        font-weight: 700;
        background: rgba(99, 102, 241, 0.15);
        color: #a5b4fc;
        padding: 1px 6px;
        border-radius: 4px;
      }

      .meta-dot {
        color: var(--text-muted, #64748b);
      }

      .comment-count-text {
        color: var(--text-secondary, #94a3b8);
      }
    }
  }

  .views-metric-box {
    text-align: right;
    flex-shrink: 0;

    .views-num {
      font-family: var(--font-mono, monospace);
      font-size: 13.5px;
      font-weight: 800;
      color: var(--text-primary, #dae2fd);
    }

    .views-label {
      font-family: var(--font-mono, monospace);
      font-size: 10px;
      color: var(--text-muted, #94a3b8);
    }
  }
}

.table-loading, .empty-posts {
  padding: 30px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: var(--text-muted, #94a3b8);
  font-size: 13px;
  gap: 8px;
}
</style>
