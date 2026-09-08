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
              <q-icon name="place" size="20px" class="meta-icon" />
              <div class="meta-content">
                <small>{{ text.locationLabel }}</small>
                <strong>{{ aboutData.profile.location }}</strong>
              </div>
            </div>

            <div class="quick-meta-item">
              <q-icon name="email" size="20px" class="meta-icon" />
              <div class="meta-content">
                <small>{{ text.email }}</small>
                <a :href="`mailto:${aboutData.profile.email}`">{{ aboutData.profile.email }}</a>
              </div>
            </div>

            <div v-if="aboutData.profile.phone" class="quick-meta-item">
              <q-icon name="phone" size="20px" class="meta-icon" />
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
              <q-icon name="description" size="16px" />
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
  background: #081126;
  margin: 0 auto;
  max-width: 1060px;
}

.about-page-header {
  padding: 50px 78px 40px;
}

.about-breadcrumb {
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
}

.bc-link:hover {
  transform: translateX(-3px);
}

.bc-separator {
  color: #384f6e;
}

.bc-current {
  color: #93a9c2;
  font-size: 0.82rem;
  font-weight: 600;
}

.header-main-grid {
  display: grid;
  gap: 36px;
  grid-template-columns: 1.4fr 1fr;
  align-items: center;
}

.eyebrow-tag {
  align-items: center;
  background: rgba(70, 224, 175, 0.08);
  border: 1px solid rgba(70, 224, 175, 0.25);
  border-radius: 20px;
  color: #46e0af;
  display: inline-flex;
  font-size: 0.75rem;
  font-weight: 700;
  gap: 8px;
  letter-spacing: 0.08em;
  padding: 6px 14px;
  text-transform: uppercase;
}

.pulse-dot {
  background: #46e0af;
  border-radius: 50%;
  box-shadow: 0 0 8px #46e0af;
  display: inline-block;
  height: 7px;
  width: 7px;
}

.page-title {
  color: #f1f4ff;
  font-size: clamp(2.4rem, 4vw, 3.5rem);
  font-weight: 900;
  letter-spacing: -0.05em;
  line-height: 1.1;
  margin: 16px 0 8px;
}

.page-subtitle {
  color: #46e0af;
  font-size: 1.25rem;
  font-weight: 800;
  letter-spacing: -0.02em;
  margin: 0 0 16px;
}

.page-lead {
  color: #b9cce0;
  font-size: 0.98rem;
  line-height: 1.7;
  margin: 0;
}

.header-quick-card {
  background: #111b33;
  border: 1px solid #233556;
  border-radius: 6px;
  display: flex;
  flex-direction: column;
  gap: 16px;
  padding: 24px 26px;
}

.quick-meta-item {
  align-items: center;
  display: flex;
  gap: 14px;
}

.meta-icon {
  color: #35d6ff;
  flex-shrink: 0;
}

.meta-content {
  display: flex;
  flex-direction: column;
}

.meta-content small {
  color: #7d96b0;
  font-size: 0.7rem;
  font-weight: 700;
  letter-spacing: 0.05em;
  text-transform: uppercase;
}

.meta-content strong,
.meta-content span,
.meta-content a {
  color: #e4eefa;
  font-size: 0.88rem;
  font-weight: 600;
  text-decoration: none;
}

.meta-content a:hover {
  color: #46e0af;
}

.quick-cv-btn {
  align-items: center;
  background: #46e0af;
  border-radius: 4px;
  color: #071126;
  display: inline-flex;
  font-size: 0.78rem;
  font-weight: 800;
  gap: 8px;
  justify-content: center;
  margin-top: 6px;
  padding: 10px 16px;
  text-decoration: none;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.quick-cv-btn:hover {
  box-shadow: 0 6px 18px rgba(70, 224, 175, 0.35);
  transform: translateY(-2px);
}

.load-state {
  align-items: center;
  background: #081126;
  color: #b9cce0;
  display: flex;
  gap: 14px;
  justify-content: center;
  margin: 0 auto;
  max-width: 1060px;
  min-height: 64vh;
  padding: 24px;
}

@media (max-width: 760px) {
  .about-page-header {
    padding: 40px 24px 30px;
  }

  .header-main-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 500px) {
  .about-page-header {
    padding: 28px 16px 20px;
  }
  .page-title {
    font-size: 2.2rem;
  }
  .header-quick-card {
    padding: 18px 16px;
    gap: 12px;
  }
}

:global(body.portfolio-light) .portfolio-content {
  .about-breadcrumb {
    .bc-link {
      color: #0f9f74;
    }
    .bc-separator {
      color: #8da1b9;
    }
    .bc-current {
      color: #4a5e7b;
    }
  }

  .eyebrow-tag {
    background: rgba(15, 159, 116, 0.1);
    border-color: rgba(15, 159, 116, 0.35);
    color: #0f9f74;

    .pulse-dot {
      background: #0f9f74;
      box-shadow: 0 0 8px rgba(15, 159, 116, 0.6);
    }
  }

  .page-title {
    color: #0a1733;
  }

  .page-subtitle {
    color: #0f9f74;
  }

  .page-lead {
    color: #354764;
  }

  .header-quick-card {
    background: #ffffff;
    border-color: #ccd7e6;
    box-shadow: 0 10px 30px rgba(21, 33, 60, 0.08);

    .meta-icon {
      color: #0b8062;
    }

    .meta-content small {
      color: #627794;
    }

    .meta-content strong,
    .meta-content span,
    .meta-content a {
      color: #0c1833;
    }

    .meta-content a:hover {
      color: #0f9f74;
    }
  }
}
</style>
