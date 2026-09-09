<template>
  <q-page class="portfolio-page">
    <main v-if="aboutData" class="portfolio-content">
      <!-- 1. Elegant About Page Header -->
      <header class="about-page-header">
        <nav class="about-breadcrumb">
          <router-link to="/home" class="bc-link">
            <q-icon name="arrow_back" size="16px" />
            <span>{{ text.home }}</span>
          </router-link>
          <span class="bc-separator">/</span>
          <span class="bc-current">{{ text.about }}</span>
        </nav>

        <div class="header-main-grid">
          <div class="header-text-block">
            <div class="eyebrow-tag">
              <span class="pulse-dot"></span>
              {{ text.aboutPageEyebrow }} · {{ aboutData.profile.role }}
            </div>
            <h1 class="page-title">{{ aboutData.profile.name }}</h1>
            <p class="page-subtitle">{{ aboutData.profile.jobTitle || 'Fullstack Web Developer' }}</p>
            <p class="page-lead">{{ aboutData.profile.bio || text.aboutPageBio }}</p>
          </div>

          <div class="header-quick-card">
            <div v-if="aboutData.profile.location" class="quick-meta-item">
              <q-icon name="place" size="18px" class="meta-icon" />
              <div class="meta-content">
                <small>{{ text.locationLabel }}</small>
                <strong>{{ aboutData.profile.location }}</strong>
              </div>
            </div>

            <div class="quick-meta-item">
              <q-icon name="email" size="18px" class="meta-icon" />
              <div class="meta-content">
                <small>{{ text.email }}</small>
                <a :href="`mailto:${aboutData.profile.email}`">{{ aboutData.profile.email }}</a>
              </div>
            </div>

            <div v-if="aboutData.profile.phone" class="quick-meta-item">
              <q-icon name="phone" size="18px" class="meta-icon" />
              <div class="meta-content">
                <small>{{ text.phoneLabel }}</small>
                <span>{{ aboutData.profile.phone }}</span>
              </div>
            </div>

            <a
              v-if="aboutData.profile.cvUrl"
              :href="aboutData.profile.cvUrl"
              target="_blank"
              rel="noopener noreferrer"
              class="quick-cv-btn"
            >
              <q-icon name="description" size="15px" />
              <span>{{ text.downloadCv }}</span>
            </a>
          </div>
        </div>
      </header>

      <!-- 2. Live Stats Bar -->
      <about-stats :stats="aboutData.stats" />

      <!-- 3. Story & Philosophy -->
      <about-story :story="aboutData.profile.aboutStory" />

      <!-- 4. Skills & Tech Stack -->
      <about-skills :skills="aboutData.skills" />

      <!-- 5. Experience & Education Timeline -->
      <about-timeline
        :experiences="aboutData.experiences"
        :educations="aboutData.educations"
      />

      <!-- 6. Contact CTA -->
      <about-cta
        :email="aboutData.profile.email"
        :cv-url="aboutData.profile.cvUrl"
      />
    </main>

    <div v-else-if="errorMessage" class="load-state">
      <q-icon name="error_outline" size="32px" color="negative" />
      <span>{{ errorMessage }}</span>
    </div>

    <div v-else class="load-state">
      <q-spinner color="teal-4" size="36px" />
      <span>{{ text.loading }}</span>
    </div>

    <portfolio-footer :email="aboutData?.profile.email ?? null" />
  </q-page>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';
import api from '@/boot/ApiGateway/axios';
import AboutCta from '@/components/about/AboutCta.vue';
import AboutSkills from '@/components/about/AboutSkills.vue';
import AboutStats from '@/components/about/AboutStats.vue';
import AboutStory from '@/components/about/AboutStory.vue';
import AboutTimeline from '@/components/about/AboutTimeline.vue';
import PortfolioFooter from '@/components/portfolio/PortfolioFooter.vue';
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';
import type { PublicAboutResponse } from '@/types/public-about';

const aboutData = ref<PublicAboutResponse | null>(null);
const errorMessage = ref('');
const route = useRoute();
const { text } = usePortfolioLocale();

onMounted(async () => {
  try {
    const username = typeof route.query.username === 'string' ? route.query.username : undefined;
    const response = await api.get<PublicAboutResponse>('/public/about', {
      params: { username }
    });
    aboutData.value = response.data;
  } catch (err) {
    console.error('Failed to load about data:', err);
    errorMessage.value = text.value.loadFailed;
  }
});
</script>

<style scoped lang="scss">
.portfolio-page {
  background: transparent;
  min-height: 100vh;
}

.portfolio-content {
  margin: 0 auto;
  max-width: 1140px;
  padding: 0 32px;
}

.about-page-header {
  padding: 40px 0;
  border-bottom: 1px solid var(--border-hairline);
}

.about-breadcrumb {
  align-items: center;
  display: flex;
  gap: 10px;
  margin-bottom: 28px;
  font-family: var(--font-body);
}

.bc-link {
  align-items: center;
  color: var(--accent-primary);
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
  color: var(--text-muted);
}

.bc-current {
  color: var(--text-secondary);
  font-size: 0.82rem;
  font-weight: 600;
}

.header-main-grid {
  display: grid;
  gap: 36px;
  grid-template-columns: 1.35fr 1fr;
  align-items: center;
}

.eyebrow-tag {
  align-items: center;
  background: var(--accent-primary-container);
  border: 1px solid rgba(16, 185, 129, 0.25);
  border-radius: var(--radius-pill);
  color: var(--accent-primary);
  display: inline-flex;
  font-family: var(--font-mono);
  font-size: 0.72rem;
  font-weight: 700;
  gap: 8px;
  letter-spacing: 0.08em;
  padding: 5px 14px;
  text-transform: uppercase;
}

.pulse-dot {
  background: var(--accent-primary);
  border-radius: 50%;
  box-shadow: 0 0 8px var(--accent-primary);
  display: inline-block;
  height: 6px;
  width: 6px;
  animation: pulse-glow 2s infinite;
}

.page-title {
  font-family: var(--font-headline);
  color: var(--text-primary);
  font-size: clamp(2.4rem, 4vw, 3.5rem);
  font-weight: 800;
  letter-spacing: -0.035em;
  line-height: 1.1;
  margin: 16px 0 8px;
}

.page-subtitle {
  font-family: var(--font-headline);
  color: var(--accent-primary);
  font-size: 1.25rem;
  font-weight: 700;
  letter-spacing: -0.015em;
  margin: 0 0 16px;
}

.page-lead {
  font-family: var(--font-body);
  color: var(--text-secondary);
  font-size: 0.98rem;
  line-height: 1.7;
  margin: 0;
}

.header-quick-card {
  background: var(--bg-surface-low);
  border: 1px solid var(--border-hairline);
  border-radius: var(--radius-lg);
  display: flex;
  flex-direction: column;
  gap: 16px;
  padding: 24px 26px;
  box-shadow: var(--shadow-card);
}

.quick-meta-item {
  align-items: center;
  display: flex;
  gap: 14px;
}

.meta-icon {
  color: var(--accent-primary);
  flex-shrink: 0;
}

.meta-content {
  display: flex;
  flex-direction: column;
}

.meta-content small {
  font-family: var(--font-mono);
  color: var(--text-muted);
  font-size: 0.68rem;
  font-weight: 700;
  letter-spacing: 0.05em;
  text-transform: uppercase;
}

.meta-content strong,
.meta-content span,
.meta-content a {
  font-family: var(--font-body);
  color: var(--text-primary);
  font-size: 0.88rem;
  font-weight: 600;
  text-decoration: none;
}

.meta-content a:hover {
  color: var(--accent-primary);
}

.quick-cv-btn {
  font-family: var(--font-headline);
  align-items: center;
  background: var(--accent-primary);
  border-radius: var(--radius-md);
  color: var(--accent-on-primary);
  display: inline-flex;
  font-size: 0.82rem;
  font-weight: 700;
  gap: 8px;
  justify-content: center;
  margin-top: 6px;
  padding: 10px 16px;
  text-decoration: none;
  transition: transform 0.2s ease, box-shadow 0.2s ease;

  &:hover {
    background: var(--accent-primary-hover);
    box-shadow: 0 6px 18px rgba(16, 185, 129, 0.3);
    transform: translateY(-2px);
  }
}

.load-state {
  align-items: center;
  color: var(--text-secondary);
  display: flex;
  gap: 14px;
  justify-content: center;
  margin: 0 auto;
  max-width: 1060px;
  min-height: 64vh;
  padding: 24px;
}

@media (max-width: 860px) {
  .portfolio-content {
    padding: 0 20px;
  }

  .header-main-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 500px) {
  .portfolio-content {
    padding: 0 16px;
  }
  .page-title {
    font-size: 2.2rem;
  }
  .header-quick-card {
    padding: 18px 16px;
    gap: 12px;
  }
}
</style>
