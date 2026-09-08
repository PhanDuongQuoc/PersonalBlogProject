<template>
  <q-page class="portfolio-page">
    <main v-if="post" class="post-page-container">
      <!-- Breadcrumb & Back Bar -->
      <nav class="editorial-nav-bar">
        <router-link to="/home" class="bc-link">
          <q-icon name="arrow_back" size="16px" />
          <span>{{ text.backToHome }}</span>
        </router-link>
        <span class="bc-separator">/</span>
        <span class="bc-category">{{ post.category.name }}</span>
      </nav>

      <!-- 1. Post Header -->
      <header class="editorial-header">
        <div class="header-eyebrow-row">
          <span class="category-pill">{{ post.category.name }}</span>
          <span class="meta-dot">·</span>
          <span class="read-time">{{ calculateReadTime(post.content) }} {{ text.minRead }}</span>
          <span class="meta-dot">·</span>
          <span class="publish-date">{{ formatDate(post.publishedAt) }}</span>
          <span class="meta-dot">·</span>
          <span class="views-metric">{{ post.viewCount }} {{ text.viewsCount }}</span>
        </div>

        <h1 class="editorial-title">{{ post.title }}</h1>

        <p v-if="post.excerpt" class="editorial-lead">
          {{ post.excerpt }}
        </p>

        <div class="author-header-bar">
          <div class="author-left">
            <q-avatar size="44px" class="author-avatar">
              <img v-if="post.author.avatarUrl" :src="post.author.avatarUrl" :alt="post.author.name" />
              <q-icon v-else name="person" size="24px" />
              <q-badge floating color="teal" rounded class="verified-badge">
                <q-icon name="check" size="10px" color="white" />
              </q-badge>
            </q-avatar>
            <div class="author-text">
              <div class="name-row">
                <strong>{{ post.author.name }}</strong>
                <q-icon name="verified" size="16px" class="verified-icon" />
              </div>
              <span class="author-title">{{ post.author.jobTitle || post.author.role }}</span>
            </div>
          </div>

          <div class="author-right">
            <q-btn
              unelevated
              no-caps
              dense
              class="follow-btn"
              :class="{ 'is-following': isFollowing }"
              @click="toggleFollow"
            >
              {{ isFollowing ? 'Following' : text.followBtn }}
            </q-btn>
            <router-link to="/about" class="dossier-link">
              <span>Dossier #{{ post.author.id }}</span>
            </router-link>
          </div>
        </div>
      </header>

      <!-- 2. Main 3-Column Editorial Grid -->
      <div class="editorial-layout-grid">
        <!-- Column 1: Left Floating Action Rail -->
        <aside class="action-rail">
          <div class="action-rail-sticky">
            <!-- Like / Clap Button -->
            <button
              type="button"
              class="rail-btn like-btn"
              :class="{ 'is-active': isLiked }"
              :title="isLiked ? 'Liked' : 'Like post'"
              @click="toggleLike"
            >
              <q-icon :name="isLiked ? 'favorite' : 'favorite_border'" size="20px" />
              <span class="btn-count">{{ currentLikeCount }}</span>
            </button>

            <!-- Comment Button -->
            <button
              type="button"
              class="rail-btn comment-btn"
              title="Jump to discussion"
              @click="scrollToComments"
            >
              <q-icon name="chat_bubble_outline" size="20px" />
              <span class="btn-count">{{ post.comments.length }}</span>
            </button>

            <!-- Bookmark Button -->
            <button
              type="button"
              class="rail-btn bookmark-btn"
              :class="{ 'is-active': isBookmarked }"
              :title="isBookmarked ? 'Bookmarked' : 'Save story'"
              @click="toggleBookmark"
            >
              <q-icon :name="isBookmarked ? 'bookmark' : 'bookmark_border'" size="20px" />
            </button>

            <!-- Text Size Toggle Button -->
            <button
              type="button"
              class="rail-btn font-size-btn"
              :class="{ 'is-active': isLargeText }"
              title="Adjust reading text size"
              @click="toggleTextSize"
            >
              <span class="font-icon">Aa</span>
            </button>

            <!-- Share Button -->
            <button
              type="button"
              class="rail-btn share-btn"
              title="Share / Copy Link"
              @click="copyShareLink"
            >
              <q-icon name="share" size="18px" />
            </button>
          </div>
        </aside>

        <!-- Column 2: Center Main Reading Column -->
        <div class="reading-column">
          <!-- Featured Image Showcase -->
          <div class="featured-media-block">
            <div class="media-frame">
              <img
                v-if="post.thumbnailUrl"
                :src="post.thumbnailUrl"
                :alt="post.title"
                class="featured-image"
              />
              <div v-else class="media-placeholder-gradient">
                <q-icon name="image" size="64px" class="ph-icon" />
              </div>
            </div>
            <p class="media-caption">
              Fig. 1.0 — Architecture & technical breakdown for {{ post.title }}. System diagram and implementation archive.
            </p>
          </div>

          <!-- Main Article Content Body -->
          <article class="article-content-body" :class="{ 'large-reading-text': isLargeText }">
            <div class="prose-content" v-html="formattedContent"></div>

            <!-- Tags Row -->
            <div v-if="post.tags && post.tags.length" class="tags-section">
              <span class="tags-header">
                <q-icon name="local_offer" size="15px" />
                {{ text.tagsTitle }}:
              </span>
              <div class="tags-cloud">
                <span v-for="tag in post.tags" :key="tag.id" class="editorial-tag-pill">
                  #{{ tag.name }}
                </span>
              </div>
            </div>
          </article>

          <!-- Interactive Comments Section -->
          <section id="comments-section" class="editorial-comments-section">
            <div class="comments-section-header">
              <div class="header-left">
                <span class="section-kicker">Discussion & Feedback</span>
                <h2 class="comments-heading">{{ text.commentsSection }} ({{ post.comments.length }})</h2>
              </div>
            </div>

            <!-- Comments List -->
            <div v-if="post.comments.length" class="comments-stream">
              <div v-for="c in post.comments" :key="c.id" class="editorial-comment-card">
                <div class="comment-user-row">
                  <q-avatar size="34px" class="comment-user-avatar">
                    {{ (c.guestName || 'K').charAt(0).toUpperCase() }}
                  </q-avatar>
                  <div class="comment-user-meta">
                    <strong>{{ c.guestName || 'Bạn đọc' }}</strong>
                    <span class="comment-date">{{ formatCommentDate(c.createdAt) }}</span>
                  </div>
                </div>
                <p class="comment-text-content">{{ c.content }}</p>
              </div>
            </div>

            <div v-else class="empty-comments-state">
              <q-icon name="chat_bubble_outline" size="28px" />
              <p>{{ text.noCommentsMessage }}</p>
            </div>

            <!-- Leave Comment Form -->
            <div class="comment-composer-card">
              <h3 class="composer-title">{{ text.leaveAComment }}</h3>

              <div v-if="commentSuccess" class="composer-alert-success">
                <q-icon name="check_circle" size="18px" />
                <span>{{ text.commentSuccessMsg }}</span>
              </div>

              <form @submit.prevent="submitComment" class="composer-form">
                <div class="composer-row">
                  <div class="field-group">
                    <label>{{ text.nameField }}</label>
                    <input
                      v-model="commentForm.name"
                      type="text"
                      required
                      placeholder="e.g. John Doe"
                      class="composer-input"
                    />
                  </div>
                  <div class="field-group">
                    <label>{{ text.emailField }}</label>
                    <input
                      v-model="commentForm.email"
                      type="email"
                      placeholder="e.g. john@example.com"
                      class="composer-input"
                    />
                  </div>
                </div>

                <div class="field-group">
                  <label>{{ text.commentField }}</label>
                  <textarea
                    v-model="commentForm.content"
                    required
                    rows="4"
                    placeholder="Share your thoughts, questions or feedback on this article..."
                    class="composer-textarea"
                  ></textarea>
                </div>

                <button
                  type="submit"
                  :disabled="isSubmittingComment || !commentForm.content.trim()"
                  class="composer-submit-btn"
                >
                  <q-spinner v-if="isSubmittingComment" size="16px" color="dark" />
                  <q-icon v-else name="send" size="16px" />
                  <span>{{ isSubmittingComment ? text.postingComment : text.postCommentBtn }}</span>
                </button>
              </form>
            </div>
          </section>
        </div>

        <!-- Column 3: Right Sidebar Column -->
        <aside class="sidebar-column">
          <div class="sidebar-sticky-stack">
            <!-- Widget 1: Table of Contents -->
            <div class="sidebar-card toc-card">
              <div class="toc-header">
                <span class="toc-title">{{ text.tableOfContents }}</span>
                <span class="toc-count">{{ tocHeadings.length }} {{ text.sections }}</span>
              </div>
              <nav class="toc-nav">
                <a
                  v-for="heading in tocHeadings"
                  :key="heading.id"
                  :href="`#${heading.id}`"
                  class="toc-item"
                  :class="{ 'is-active': activeHeadingId === heading.id, 'toc-level-3': heading.level === 3 }"
                  @click.prevent="scrollToHeading(heading.id)"
                >
                  <span class="toc-dot"></span>
                  <span class="toc-text">{{ heading.text }}</span>
                </a>
              </nav>
            </div>

            <!-- Widget 2: Author Dossier Card -->
            <div class="sidebar-card author-dossier-card">
              <div class="dossier-head">
                <q-avatar size="48px" class="dossier-avatar">
                  <img v-if="post.author.avatarUrl" :src="post.author.avatarUrl" :alt="post.author.name" />
                  <q-icon v-else name="person" size="26px" />
                </q-avatar>
                <div class="dossier-author-info">
                  <strong>{{ post.author.name }}</strong>
                  <span>{{ post.author.jobTitle || 'Fullstack Web Developer' }}</span>
                </div>
              </div>
              <p class="dossier-bio">
                {{ post.author.bio || 'Investigating robust computational architectures, modern web frameworks and reliable backend systems.' }}
              </p>
              <div class="dossier-footer">
                <span class="dossier-subscribers">{{ post.viewCount * 12 + 150 }} Readers</span>
                <router-link to="/about" class="dossier-action-link">
                  <span>{{ text.viewDossier }}</span>
                  <q-icon name="arrow_forward" size="14px" />
                </router-link>
              </div>
            </div>

            <!-- Widget 3: Newsletter / Dispatch Card -->
            <div class="sidebar-card newsletter-card">
              <div class="newsletter-kicker">
                <q-icon name="mail_outline" size="16px" />
                <span>{{ text.newsletterTitle }}</span>
              </div>
              <h4 class="newsletter-heading">Get essays like this in your inbox.</h4>
              <p class="newsletter-description">
                {{ text.newsletterDesc }}
              </p>
              <form @submit.prevent="handleNewsletterSubscribe" class="newsletter-form">
                <input
                  v-model="newsletterEmail"
                  type="email"
                  required
                  placeholder="Enter your email address"
                  class="newsletter-input"
                />
                <button type="submit" class="newsletter-submit-btn">
                  {{ text.subscribeBtn }}
                </button>
              </form>
              <div v-if="newsletterSuccess" class="newsletter-success-toast">
                <q-icon name="check_circle" size="15px" />
                <span>{{ text.subscribeSuccessMsg }}</span>
              </div>
            </div>

            <!-- Widget 4: Trending in Category / Related Posts -->
            <div v-if="post.relatedPosts && post.relatedPosts.length" class="sidebar-card trending-card">
              <div class="trending-header">
                <span class="trending-kicker">Trending in {{ post.category.name }}</span>
              </div>
              <div class="trending-list">
                <router-link
                  v-for="rel in post.relatedPosts"
                  :key="rel.id"
                  :to="`/posts/${rel.slug}`"
                  class="trending-item"
                >
                  <span class="trending-meta">ESSAY · 5 MIN</span>
                  <h5 class="trending-title">{{ rel.title }}</h5>
                  <div class="trending-author-row">
                    <span class="trending-date">{{ formatDate(rel.publishedAt) }}</span>
                  </div>
                </router-link>
              </div>
            </div>
          </div>
        </aside>
      </div>
    </main>

    <!-- Error / Loading States -->
    <div v-else-if="errorMessage" class="editorial-load-state">
      <q-icon name="error_outline" size="48px" color="negative" />
      <p>{{ errorMessage }}</p>
      <router-link to="/home" class="return-home-btn">{{ text.backToHome }}</router-link>
    </div>

    <div v-else class="editorial-load-state">
      <q-spinner color="teal" size="44px" />
      <p>{{ text.loading }}</p>
    </div>

    <!-- Toast Notification for Share Link -->
    <q-dialog v-model="showShareToast" position="top">
      <div class="share-toast-banner">
        <q-icon name="check_circle" size="18px" color="teal-3" />
        <span>{{ text.copiedLink }}</span>
      </div>
    </q-dialog>

    <portfolio-footer :email="post?.author.email ?? null" />
  </q-page>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import { useRoute } from 'vue-router';
import api from '@/boot/ApiGateway/axios';
import PortfolioFooter from '@/components/portfolio/PortfolioFooter.vue';
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';
import type { PublicPostDetailResponse } from '@/types/public-post';

interface TocHeading {
  id: string;
  text: string;
  level: number;
}

const route = useRoute();
const { locale, text } = usePortfolioLocale();

const post = ref<PublicPostDetailResponse | null>(null);
const errorMessage = ref('');
const isSubmittingComment = ref(false);
const commentSuccess = ref(false);

const isLiked = ref(false);
const likeCountOffset = ref(0);
const isBookmarked = ref(false);
const isFollowing = ref(false);
const isLargeText = ref(false);
const showShareToast = ref(false);

const newsletterEmail = ref('');
const newsletterSuccess = ref(false);

const tocHeadings = ref<TocHeading[]>([]);
const activeHeadingId = ref('');

const commentForm = ref({
  name: '',
  email: '',
  content: ''
});

const currentLikeCount = computed(() => {
  const base = post.value?.viewCount || 12;
  return base + likeCountOffset.value;
});

const toggleLike = () => {
  isLiked.value = !isLiked.value;
  likeCountOffset.value += isLiked.value ? 1 : -1;
};

const toggleBookmark = () => {
  isBookmarked.value = !isBookmarked.value;
};

const toggleFollow = () => {
  isFollowing.value = !isFollowing.value;
};

const toggleTextSize = () => {
  isLargeText.value = !isLargeText.value;
};

const copyShareLink = async () => {
  try {
    if (typeof window !== 'undefined') {
      await navigator.clipboard.writeText(window.location.href);
      showShareToast.value = true;
      setTimeout(() => {
        showShareToast.value = false;
      }, 3000);
    }
  } catch {
    // Fallback
  }
};

const scrollToComments = () => {
  const el = document.getElementById('comments-section');
  if (el) {
    el.scrollIntoView({ behavior: 'smooth' });
  }
};

const scrollToHeading = (id: string) => {
  activeHeadingId.value = id;
  const el = document.getElementById(id);
  if (el) {
    el.scrollIntoView({ behavior: 'smooth' });
  }
};

const handleNewsletterSubscribe = () => {
  if (!newsletterEmail.value) return;
  newsletterSuccess.value = true;
  newsletterEmail.value = '';
  setTimeout(() => {
    newsletterSuccess.value = false;
  }, 4500);
};

const extractHeadings = () => {
  if (!post.value || !post.value.content) return;
  const headings: TocHeading[] = [];
  const lines = post.value.content.split('\n');
  let index = 1;

  for (const line of lines) {
    const trimmed = line.trim();
    if (trimmed.startsWith('# ') || trimmed.startsWith('## ') || trimmed.startsWith('### ')) {
      const level = trimmed.startsWith('# ') ? 2 : trimmed.startsWith('## ') ? 2 : 3;
      const title = trimmed.replace(/^#+\s*/, '');
      const slugId = `section-${index}-${title.toLowerCase().replace(/[^a-z0-9]+/g, '-')}`;
      headings.push({ id: slugId, text: title, level });
      index++;
    }
  }

  if (!headings.length) {
    headings.push(
      { id: 'section-1-overview', text: locale.value === 'vi' ? '1. Tổng quan kiến trúc & ý tưởng' : '1. Overview & Core Architecture', level: 2 },
      { id: 'section-2-implementation', text: locale.value === 'vi' ? '2. Triển khai kỹ thuật chuyên sâu' : '2. In-Depth Engineering Details', level: 2 },
      { id: 'section-3-summary', text: locale.value === 'vi' ? '3. Tổng kết & Đúc kết thực tế' : '3. Key Takeaways & Conclusion', level: 2 }
    );
  }

  tocHeadings.value = headings;
  if (headings.length) {
    activeHeadingId.value = headings[0].id;
  }
};

const loadPost = async (slug: string) => {
  try {
    errorMessage.value = '';
    post.value = null;
    const response = await api.get<PublicPostDetailResponse>(`/public/posts/${slug}`);
    post.value = response.data;
    extractHeadings();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  } catch (err) {
    console.error('Failed to load post detail:', err);
    errorMessage.value = 'Không thể tải bài viết này hoặc bài viết không tồn tại.';
  }
};

onMounted(() => {
  const slug = route.params.slug as string;
  if (slug) {
    loadPost(slug);
  }
});

watch(
  () => route.params.slug,
  (newSlug) => {
    if (newSlug) {
      loadPost(newSlug as string);
    }
  }
);

const formatDate = (dateString: string | null | undefined) => {
  if (!dateString) return text.value.recent;
  return new Intl.DateTimeFormat(locale.value === 'vi' ? 'vi-VN' : 'en-US', {
    day: 'numeric',
    month: 'short',
    year: 'numeric'
  }).format(new Date(dateString));
};

const formatCommentDate = (dateString: string) => {
  return new Intl.DateTimeFormat(locale.value === 'vi' ? 'vi-VN' : 'en-US', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(dateString));
};

const calculateReadTime = (content: string | undefined): number => {
  if (!content) return 1;
  const words = content.trim().split(/\s+/).length;
  return Math.max(1, Math.ceil(words / 200));
};

const formattedContent = computed(() => {
  if (!post.value || !post.value.content) return '';
  let content = post.value.content;

  if (content.includes('<p>') || content.includes('<div>')) {
    return content;
  }

  const paragraphs = content.split(/\n\n+/);
  let headingIndex = 1;

  return paragraphs
    .map((p, idx) => {
      let trimmed = p.trim();
      if (!trimmed) return '';

      if (trimmed.startsWith('# ')) {
        const title = trimmed.substring(2);
        const slugId = `section-${headingIndex}-${title.toLowerCase().replace(/[^a-z0-9]+/g, '-')}`;
        headingIndex++;
        return `<h2 id="${slugId}" class="article-h2">${title}</h2>`;
      }
      if (trimmed.startsWith('## ')) {
        const title = trimmed.substring(3);
        const slugId = `section-${headingIndex}-${title.toLowerCase().replace(/[^a-z0-9]+/g, '-')}`;
        headingIndex++;
        return `<h2 id="${slugId}" class="article-h2">${title}</h2>`;
      }
      if (trimmed.startsWith('### ')) {
        const title = trimmed.substring(4);
        const slugId = `section-${headingIndex}-${title.toLowerCase().replace(/[^a-z0-9]+/g, '-')}`;
        headingIndex++;
        return `<h3 id="${slugId}" class="article-h3">${title}</h3>`;
      }
      if (trimmed.startsWith('> ')) {
        return `<blockquote class="article-blockquote">${trimmed.substring(2)}</blockquote>`;
      }

      // First paragraph gets drop cap styling
      if (idx === 0) {
        return `<p class="article-p drop-cap-p">${trimmed.replace(/\n/g, '<br/>')}</p>`;
      }
      return `<p class="article-p">${trimmed.replace(/\n/g, '<br/>')}</p>`;
    })
    .join('');
});

const submitComment = async () => {
  if (!post.value || !commentForm.value.content.trim()) return;

  try {
    isSubmittingComment.value = true;
    commentSuccess.value = false;

    const response = await api.post(
      `/public/posts/${post.value.slug}/comments`,
      commentForm.value
    );

    if (response.data) {
      post.value.comments.unshift(response.data);
      commentForm.value = { name: '', email: '', content: '' };
      commentSuccess.value = true;
      setTimeout(() => {
        commentSuccess.value = false;
      }, 5000);
    }
  } catch (err) {
    console.error('Failed to submit comment:', err);
  } finally {
    isSubmittingComment.value = false;
  }
};
</script>

<style scoped lang="scss">
.portfolio-page {
  background: transparent;
  min-height: 100vh;
}

.post-page-container {
  background: #081126;
  margin: 0 auto;
  max-width: 1180px;
  padding: 40px 36px 90px;
}

// 1. Editorial Navigation Bar
.editorial-nav-bar {
  align-items: center;
  display: flex;
  gap: 10px;
  margin-bottom: 28px;
}

.bc-link {
  align-items: center;
  color: #46e0af;
  display: inline-flex;
  font-size: 0.82rem;
  font-weight: 700;
  gap: 6px;
  text-decoration: none;
  transition: transform 0.2s ease;

  &:hover {
    transform: translateX(-3px);
  }
}

.bc-separator {
  color: #2e4162;
}

.bc-category {
  color: #8da1b9;
  font-size: 0.82rem;
  font-weight: 600;
}

// 2. Editorial Header
.editorial-header {
  border-bottom: 1px solid #1c2b4a;
  margin-bottom: 44px;
  padding-bottom: 36px;
}

.header-eyebrow-row {
  align-items: center;
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  margin-bottom: 18px;
}

.category-pill {
  background: rgba(70, 224, 175, 0.1);
  border: 1px solid rgba(70, 224, 175, 0.3);
  border-radius: 4px;
  color: #46e0af;
  font-size: 0.72rem;
  font-weight: 800;
  letter-spacing: 0.08em;
  padding: 4px 10px;
  text-transform: uppercase;
}

.meta-dot {
  color: #3b5072;
}

.read-time,
.publish-date,
.views-metric {
  color: #8da1b9;
  font-size: 0.82rem;
  font-weight: 600;
}

.editorial-title {
  color: #f1f4ff;
  font-family: inherit;
  font-size: clamp(2.3rem, 4.4vw, 3.6rem);
  font-weight: 900;
  letter-spacing: -0.04em;
  line-height: 1.15;
  margin: 0 0 18px;
}

.editorial-lead {
  color: #9db3cc;
  font-size: clamp(1.05rem, 1.8vw, 1.25rem);
  font-weight: 400;
  line-height: 1.7;
  margin: 0 0 28px;
  max-width: 960px;
}

.author-header-bar {
  align-items: center;
  display: flex;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 16px;
  padding-top: 10px;
}

.author-left {
  align-items: center;
  display: flex;
  gap: 14px;
}

.author-avatar {
  background: #15223e;
  border: 2px solid #283e66;
  position: relative;
}

.verified-badge {
  bottom: -2px;
  padding: 2px;
  right: -2px;
}

.author-text {
  display: flex;
  flex-direction: column;
}

.name-row {
  align-items: center;
  display: flex;
  gap: 6px;

  strong {
    color: #f1f4ff;
    font-size: 0.96rem;
    font-weight: 700;
  }
}

.verified-icon {
  color: #46e0af;
}

.author-title {
  color: #7d96b3;
  font-size: 0.78rem;
  font-weight: 500;
}

.author-right {
  align-items: center;
  display: flex;
  gap: 14px;
}

.follow-btn {
  background: rgba(70, 224, 175, 0.12);
  border: 1px solid rgba(70, 224, 175, 0.35);
  border-radius: 4px;
  color: #46e0af;
  font-size: 0.76rem;
  font-weight: 700;
  padding: 6px 16px;
  transition: all 0.2s ease;

  &:hover,
  &.is-following {
    background: #46e0af;
    color: #071126;
  }
}

.dossier-link {
  color: #7d96b3;
  font-size: 0.78rem;
  font-weight: 600;
  text-decoration: none;

  &:hover {
    color: #46e0af;
  }
}

// 3. Editorial Layout Grid (3 Columns)
.editorial-layout-grid {
  display: grid;
  gap: 36px;
  grid-template-columns: 48px minmax(0, 1fr) 310px;
  position: relative;
}

// Column 1: Left Action Rail
.action-rail {
  position: relative;
}

.action-rail-sticky {
  display: flex;
  flex-direction: column;
  gap: 14px;
  position: sticky;
  top: 100px;
}

.rail-btn {
  align-items: center;
  background: #111b33;
  border: 1px solid #233556;
  border-radius: 50%;
  color: #9db3cc;
  cursor: pointer;
  display: flex;
  flex-direction: column;
  height: 44px;
  justify-content: center;
  transition: all 0.2s ease;
  width: 44px;

  &:hover {
    background: #18284d;
    border-color: #46e0af;
    color: #46e0af;
    transform: scale(1.05);
  }

  &.is-active {
    background: rgba(70, 224, 175, 0.15);
    border-color: #46e0af;
    color: #46e0af;
  }

  .btn-count {
    font-size: 0.65rem;
    font-weight: 800;
    line-height: 1;
    margin-top: 2px;
  }

  .font-icon {
    font-size: 0.85rem;
    font-weight: 900;
  }
}

// Column 2: Reading Column
.reading-column {
  min-width: 0;
}

.featured-media-block {
  margin-bottom: 36px;
}

.media-frame {
  border-radius: 12px;
  box-shadow: 0 16px 36px rgba(0, 0, 0, 0.35);
  max-height: 480px;
  overflow: hidden;
  position: relative;
  width: 100%;
}

.featured-image {
  height: 100%;
  max-height: 480px;
  object-fit: cover;
  width: 100%;
}

.media-placeholder-gradient {
  align-items: center;
  background: linear-gradient(135deg, #15223e, #0e172a);
  border: 1px solid #233556;
  display: flex;
  height: 320px;
  justify-content: center;
  width: 100%;

  .ph-icon {
    color: #3b5072;
  }
}

.media-caption {
  color: #7d96b3;
  font-size: 0.8rem;
  font-style: italic;
  line-height: 1.5;
  margin: 12px 0 0;
  text-align: center;
}

// Article Prose Styling
.article-content-body {
  font-size: 1.05rem;
  line-height: 1.85;

  &.large-reading-text {
    font-size: 1.2rem;
    line-height: 1.95;
  }
}

.prose-content {
  :deep(.drop-cap-p::first-letter) {
    color: #46e0af;
    float: left;
    font-family: Georgia, serif;
    font-size: 3.6rem;
    font-weight: 900;
    line-height: 0.8;
    margin-right: 10px;
    padding-top: 4px;
  }

  :deep(.article-p) {
    color: #c4d6ea;
    margin-bottom: 24px;
  }

  :deep(.article-h2) {
    color: #f1f4ff;
    font-size: 1.65rem;
    font-weight: 800;
    letter-spacing: -0.02em;
    line-height: 1.3;
    margin: 44px 0 18px;
    padding-top: 14px;
    scroll-margin-top: 100px;
  }

  :deep(.article-h3) {
    color: #e2ecfa;
    font-size: 1.35rem;
    font-weight: 700;
    margin: 32px 0 14px;
    scroll-margin-top: 100px;
  }

  :deep(.article-blockquote) {
    background: #0f1a30;
    border-left: 3px solid #46e0af;
    border-radius: 0 8px 8px 0;
    color: #dce8fa;
    font-style: italic;
    margin: 30px 0;
    padding: 18px 24px;
  }
}

// Tags Section
.tags-section {
  align-items: center;
  border-top: 1px solid #1c2b4a;
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  margin: 44px 0 36px;
  padding-top: 24px;
}

.tags-header {
  align-items: center;
  color: #7d96b3;
  display: inline-flex;
  font-size: 0.85rem;
  font-weight: 700;
  gap: 6px;
}

.tags-cloud {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.editorial-tag-pill {
  background: #111b33;
  border: 1px solid #233556;
  border-radius: 4px;
  color: #46e0af;
  font-size: 0.78rem;
  font-weight: 600;
  padding: 4px 10px;
  transition: all 0.2s ease;

  &:hover {
    background: #18284d;
    border-color: #46e0af;
  }
}

// Comments Stream
.editorial-comments-section {
  border-top: 1px solid #1c2b4a;
  margin-top: 48px;
  padding-top: 36px;
}

.comments-section-header {
  margin-bottom: 24px;
}

.section-kicker {
  color: #46e0af;
  font-size: 0.75rem;
  font-weight: 800;
  letter-spacing: 0.08em;
  text-transform: uppercase;
}

.comments-heading {
  color: #f1f4ff;
  font-size: 1.5rem;
  font-weight: 800;
  margin: 6px 0 0;
}

.comments-stream {
  display: flex;
  flex-direction: column;
  gap: 14px;
  margin-bottom: 32px;
}

.editorial-comment-card {
  background: #0f1a30;
  border: 1px solid #1e2c4d;
  border-radius: 8px;
  padding: 18px 20px;
}

.comment-user-row {
  align-items: center;
  display: flex;
  gap: 10px;
  margin-bottom: 10px;
}

.comment-user-avatar {
  background: #46e0af;
  color: #071126;
  font-size: 0.8rem;
  font-weight: 800;
}

.comment-user-meta {
  display: flex;
  flex-direction: column;

  strong {
    color: #f1f4ff;
    font-size: 0.88rem;
    font-weight: 700;
  }
}

.comment-date {
  color: #6d85a3;
  font-size: 0.72rem;
}

.comment-text-content {
  color: #b9cce0;
  font-size: 0.92rem;
  line-height: 1.6;
  margin: 0;
}

.empty-comments-state {
  align-items: center;
  background: #0e172a;
  border: 1px dashed #233556;
  border-radius: 8px;
  color: #7d96b3;
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin-bottom: 32px;
  padding: 36px 20px;
  text-align: center;
}

.comment-composer-card {
  background: #0f1a30;
  border: 1px solid #1e2c4d;
  border-radius: 8px;
  padding: 24px;
}

.composer-title {
  color: #f1f4ff;
  font-size: 1.15rem;
  font-weight: 800;
  margin: 0 0 18px;
}

.composer-alert-success {
  align-items: center;
  background: rgba(70, 224, 175, 0.12);
  border: 1px solid #46e0af;
  border-radius: 4px;
  color: #46e0af;
  display: flex;
  font-size: 0.85rem;
  font-weight: 600;
  gap: 8px;
  margin-bottom: 16px;
  padding: 10px 14px;
}

.composer-form {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.composer-row {
  display: grid;
  gap: 16px;
  grid-template-columns: 1fr 1fr;
}

.field-group {
  display: flex;
  flex-direction: column;
  gap: 6px;

  label {
    color: #9db3cc;
    font-size: 0.78rem;
    font-weight: 700;
  }
}

.composer-input,
.composer-textarea {
  background: #081126;
  border: 1px solid #233556;
  border-radius: 4px;
  color: #f1f4ff;
  font-family: inherit;
  font-size: 0.88rem;
  padding: 10px 14px;
  transition: border-color 0.2s ease;

  &:focus {
    border-color: #46e0af;
    outline: none;
  }
}

.composer-submit-btn {
  align-items: center;
  align-self: flex-start;
  background: #46e0af;
  border: none;
  border-radius: 4px;
  color: #071126;
  cursor: pointer;
  display: inline-flex;
  font-family: inherit;
  font-size: 0.82rem;
  font-weight: 800;
  gap: 8px;
  padding: 10px 22px;
  transition: all 0.2s ease;

  &:hover:not(:disabled) {
    box-shadow: 0 4px 16px rgba(70, 224, 175, 0.35);
    transform: translateY(-2px);
  }

  &:disabled {
    cursor: not-allowed;
    opacity: 0.5;
  }
}

// Column 3: Sidebar
.sidebar-column {
  position: relative;
}

.sidebar-sticky-stack {
  display: flex;
  flex-direction: column;
  gap: 24px;
  position: sticky;
  top: 100px;
}

.sidebar-card {
  background: #0f1a30;
  border: 1px solid #1e2c4d;
  border-radius: 8px;
  padding: 20px;
}

// Widget 1: TOC
.toc-header {
  align-items: center;
  border-bottom: 1px solid #1e2c4d;
  display: flex;
  justify-content: space-between;
  margin-bottom: 14px;
  padding-bottom: 10px;
}

.toc-title {
  color: #f1f4ff;
  font-size: 0.78rem;
  font-weight: 800;
  letter-spacing: 0.08em;
  text-transform: uppercase;
}

.toc-count {
  color: #6d85a3;
  font-size: 0.72rem;
  font-weight: 600;
}

.toc-nav {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.toc-item {
  align-items: center;
  color: #8da1b9;
  display: flex;
  font-size: 0.8rem;
  font-weight: 600;
  gap: 10px;
  line-height: 1.4;
  text-decoration: none;
  transition: all 0.2s ease;

  &:hover {
    color: #46e0af;
    transform: translateX(3px);
  }

  &.is-active {
    color: #46e0af;
    font-weight: 700;

    .toc-dot {
      background: #46e0af;
      box-shadow: 0 0 6px #46e0af;
    }
  }

  &.toc-level-3 {
    padding-left: 14px;
  }
}

.toc-dot {
  background: #3b5072;
  border-radius: 50%;
  flex-shrink: 0;
  height: 6px;
  transition: all 0.2s ease;
  width: 6px;
}

// Widget 2: Author Dossier
.dossier-head {
  align-items: center;
  display: flex;
  gap: 12px;
  margin-bottom: 12px;
}

.dossier-avatar {
  background: #15223e;
  border: 1px solid #283e66;
}

.dossier-author-info {
  display: flex;
  flex-direction: column;

  strong {
    color: #f1f4ff;
    font-size: 0.92rem;
  }

  span {
    color: #7d96b3;
    font-size: 0.75rem;
  }
}

.dossier-bio {
  color: #9db3cc;
  font-size: 0.82rem;
  line-height: 1.55;
  margin: 0 0 16px;
}

.dossier-footer {
  align-items: center;
  border-top: 1px solid #1e2c4d;
  display: flex;
  justify-content: space-between;
  padding-top: 12px;
}

.dossier-subscribers {
  color: #6d85a3;
  font-size: 0.72rem;
  font-weight: 600;
}

.dossier-action-link {
  align-items: center;
  color: #46e0af;
  display: inline-flex;
  font-size: 0.75rem;
  font-weight: 700;
  gap: 4px;
  text-decoration: none;

  &:hover {
    text-decoration: underline;
  }
}

// Widget 3: Newsletter
.newsletter-kicker {
  align-items: center;
  color: #46e0af;
  display: flex;
  font-size: 0.72rem;
  font-weight: 800;
  gap: 6px;
  letter-spacing: 0.06em;
  margin-bottom: 8px;
  text-transform: uppercase;
}

.newsletter-heading {
  color: #f1f4ff;
  font-size: 0.95rem;
  font-weight: 800;
  line-height: 1.35;
  margin: 0 0 8px;
}

.newsletter-description {
  color: #8da1b9;
  font-size: 0.78rem;
  line-height: 1.5;
  margin: 0 0 14px;
}

.newsletter-form {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.newsletter-input {
  background: #081126;
  border: 1px solid #233556;
  border-radius: 4px;
  color: #f1f4ff;
  font-family: inherit;
  font-size: 0.8rem;
  padding: 8px 12px;

  &:focus {
    border-color: #46e0af;
    outline: none;
  }
}

.newsletter-submit-btn {
  background: #46e0af;
  border: none;
  border-radius: 4px;
  color: #071126;
  cursor: pointer;
  font-family: inherit;
  font-size: 0.78rem;
  font-weight: 800;
  padding: 9px;
  transition: all 0.2s ease;

  &:hover {
    box-shadow: 0 4px 14px rgba(70, 224, 175, 0.35);
  }
}

.newsletter-success-toast {
  align-items: center;
  color: #46e0af;
  display: flex;
  font-size: 0.75rem;
  font-weight: 700;
  gap: 6px;
  margin-top: 10px;
}

// Widget 4: Trending
.trending-header {
  border-bottom: 1px solid #1e2c4d;
  margin-bottom: 12px;
  padding-bottom: 8px;
}

.trending-kicker {
  color: #f1f4ff;
  font-size: 0.76rem;
  font-weight: 800;
  letter-spacing: 0.06em;
  text-transform: uppercase;
}

.trending-list {
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.trending-item {
  display: flex;
  flex-direction: column;
  gap: 4px;
  text-decoration: none;

  &:hover .trending-title {
    color: #46e0af;
  }
}

.trending-meta {
  color: #46e0af;
  font-size: 0.68rem;
  font-weight: 800;
  letter-spacing: 0.05em;
}

.trending-title {
  color: #dce8fa;
  font-size: 0.85rem;
  font-weight: 700;
  line-height: 1.35;
  margin: 0;
  transition: color 0.2s ease;
}

.trending-date {
  color: #6d85a3;
  font-size: 0.72rem;
}

// Loading & Share Toast
.editorial-load-state {
  align-items: center;
  color: #8da1b9;
  display: flex;
  flex-direction: column;
  gap: 16px;
  justify-content: center;
  min-height: 60vh;
  padding: 40px;
  text-align: center;
}

.return-home-btn {
  background: #46e0af;
  border-radius: 4px;
  color: #071126;
  font-size: 0.8rem;
  font-weight: 800;
  padding: 8px 18px;
  text-decoration: none;
}

.share-toast-banner {
  align-items: center;
  background: #0f1a30;
  border: 1px solid #46e0af;
  border-radius: 8px;
  color: #f1f4ff;
  display: flex;
  font-size: 0.85rem;
  font-weight: 700;
  gap: 10px;
  padding: 12px 22px;
}

// Responsive Design
@media (max-width: 1024px) {
  .editorial-layout-grid {
    grid-template-columns: 48px 1fr;
  }

  .sidebar-column {
    display: none;
  }
}

@media (max-width: 760px) {
  .post-page-container {
    padding: 30px 20px 60px;
  }

  .editorial-layout-grid {
    display: block;
  }

  .action-rail {
    margin-bottom: 24px;
  }

  .action-rail-sticky {
    flex-direction: row;
    position: static;
    flex-wrap: wrap;
    gap: 10px;
  }

  .composer-row {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 500px) {
  .post-page-container {
    padding: 20px 14px 48px;
  }

  .editorial-title {
    font-size: 1.85rem;
    margin-bottom: 12px;
  }

  .editorial-lead {
    font-size: 0.92rem;
    margin-bottom: 20px;
  }

  .rail-btn {
    width: 38px;
    height: 38px;
  }

  .comment-composer-card {
    padding: 16px 14px;
  }

  .editorial-comment-card {
    padding: 14px 12px;
  }
}
</style>

<style lang="scss">
// Global Light Mode Override for Editorial Post Detail
body.portfolio-light {
  .post-page-container {
    background: #ffffff !important;
  }

  .editorial-nav-bar {
    .bc-link {
      color: #0f9f74 !important;
    }
    .bc-separator {
      color: #cbd5e1 !important;
    }
    .bc-category {
      color: #475569 !important;
    }
  }

  .editorial-header {
    border-color: #eaedf3 !important;
  }

  .category-pill {
    background: rgba(15, 159, 116, 0.1) !important;
    border-color: rgba(15, 159, 116, 0.3) !important;
    color: #0f9f74 !important;
  }

  .read-time,
  .publish-date,
  .views-metric {
    color: #64748b !important;
  }

  .editorial-title {
    color: #000000 !important;
  }

  .editorial-lead {
    color: #334155 !important;
  }

  .name-row strong {
    color: #000000 !important;
  }

  .verified-icon {
    color: #0f9f74 !important;
  }

  .author-title {
    color: #64748b !important;
  }

  .follow-btn {
    background: rgba(15, 159, 116, 0.1) !important;
    border-color: rgba(15, 159, 116, 0.35) !important;
    color: #0f9f74 !important;

    &:hover,
    &.is-following {
      background: #0f9f74 !important;
      color: #ffffff !important;
    }
  }

  .rail-btn {
    background: #ffffff !important;
    border-color: #e2e8f0 !important;
    color: #475569 !important;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);

    &:hover,
    &.is-active {
      background: #f0fdf4 !important;
      border-color: #0f9f74 !important;
      color: #0f9f74 !important;
    }
  }

  .media-caption {
    color: #64748b !important;
  }

  .prose-content {
    .drop-cap-p::first-letter {
      color: #0f9f74 !important;
    }
    .article-p {
      color: #1e293b !important;
    }
    .article-h2 {
      color: #000000 !important;
    }
    .article-h3 {
      color: #0f172a !important;
    }
    .article-blockquote {
      background: #f8fafc !important;
      border-color: #0f9f74 !important;
      color: #1e293b !important;
    }
  }

  .tags-section {
    border-color: #eaedf3 !important;
  }

  .editorial-tag-pill {
    background: #f1f5f9 !important;
    border-color: #e2e8f0 !important;
    color: #0f9f74 !important;
  }

  .editorial-comments-section {
    border-color: #eaedf3 !important;
  }

  .comments-heading {
    color: #000000 !important;
  }

  .editorial-comment-card,
  .comment-composer-card {
    background: #ffffff !important;
    border-color: #eaedf3 !important;
    box-shadow: 0 4px 18px rgba(0, 0, 0, 0.04) !important;
  }

  .comment-user-meta strong,
  .composer-title {
    color: #000000 !important;
  }

  .comment-text-content {
    color: #334155 !important;
  }

  .composer-input,
  .composer-textarea {
    background: #f8fafc !important;
    border-color: #cbd5e1 !important;
    color: #000000 !important;

    &:focus {
      background: #ffffff !important;
      border-color: #0f9f74 !important;
    }
  }

  .sidebar-card {
    background: #ffffff !important;
    border-color: #eaedf3 !important;
    box-shadow: 0 4px 18px rgba(0, 0, 0, 0.04) !important;
  }

  .toc-header,
  .dossier-footer,
  .trending-header {
    border-color: #eaedf3 !important;
  }

  .toc-title,
  .dossier-author-info strong,
  .newsletter-heading,
  .trending-kicker {
    color: #000000 !important;
  }

  .toc-item {
    color: #475569 !important;

    &:hover,
    &.is-active {
      color: #0f9f74 !important;
    }
  }

  .trending-title {
    color: #0f172a !important;
    &:hover {
      color: #0f9f74 !important;
    }
  }

  .newsletter-input {
    background: #f8fafc !important;
    border-color: #cbd5e1 !important;
    color: #000000 !important;

    &:focus {
      border-color: #0f9f74 !important;
    }
  }
}
</style>
