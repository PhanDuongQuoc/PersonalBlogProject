<template>
  <section id="top" class="hero-section">
    <div class="hero-container">
      <!-- Left Column: Editorial Positioning & Story -->
      <div class="hero-content">
        <div class="hero-eyebrow">
          <span class="status-indicator"></span>
          <span class="eyebrow-role">{{ profile.role }}</span>
          <span class="eyebrow-sep">·</span>
          <span class="eyebrow-user">@{{ profile.username }}</span>
        </div>

        <p class="hero-greeting">{{ heroText.greeting }}</p>

        <h1 class="hero-name">{{ profile.name }}</h1>

        <p class="hero-slogan">
          <span class="hero-slogan-prefix">{{ heroText.prefix }}</span>
          <span class="typewriter-gap">&nbsp;</span>
          <span class="typewriter-text">{{ displayedRole }}</span>
          <span class="typewriter-cursor">|</span>
        </p>

        <p class="hero-statement">
          {{ profile.bio || text.heroDescription.replace('{name}', profile.name) }}
        </p>

        <!-- Key Meta: Location & Email -->
        <div class="hero-meta-strip">
          <div v-if="profile.location" class="meta-entry">
            <q-icon name="place" size="16px" class="meta-icon" />
            <span>{{ profile.location }}</span>
          </div>
          <div class="meta-entry">
            <q-icon name="mail_outline" size="16px" class="meta-icon" />
            <a :href="`mailto:${profile.email}`" class="meta-link">{{ profile.email }}</a>
          </div>
        </div>

        <!-- Action CTAs -->
        <div class="hero-cta-group">
          <a
            v-if="profile.cvUrl"
            :href="profile.cvUrl"
            target="_blank"
            class="cta-primary-btn"
          >
            <span>{{ text.downloadCv }}</span>
            <q-icon name="arrow_downward" size="15px" />
          </a>
          <a
            v-else
            href="#posts"
            class="cta-primary-btn"
          >
            <span>{{ text.viewPosts }}</span>
            <q-icon name="arrow_forward" size="15px" />
          </a>

          <a href="#contact" class="cta-secondary-btn">
            <q-icon name="chat_bubble_outline" size="16px" />
            <span>{{ text.sayHello }}</span>
          </a>
        </div>

        <!-- Social Presence -->
        <div class="hero-social-strip">
          <a
            v-if="profile.githubUrl"
            :href="profile.githubUrl"
            target="_blank"
            rel="noopener noreferrer"
            class="social-link-item"
            aria-label="GitHub"
          >
            <q-icon name="code" size="16px" />
            <span>GitHub</span>
          </a>
          <a
            v-if="profile.linkedinUrl"
            :href="profile.linkedinUrl"
            target="_blank"
            rel="noopener noreferrer"
            class="social-link-item"
            aria-label="LinkedIn"
          >
            <q-icon name="public" size="16px" />
            <span>LinkedIn</span>
          </a>
          <a
            v-if="profile.websiteUrl"
            :href="profile.websiteUrl"
            target="_blank"
            rel="noopener noreferrer"
            class="social-link-item"
            aria-label="Website"
          >
            <q-icon name="link" size="16px" />
            <span>Website</span>
          </a>
        </div>
      </div>

      <!-- Right Column: Original Portrait Frame with Floating Animation & Stats -->
      <div class="hero-visual-col">
        <div class="portrait-panel">
          <div class="portrait-frame">
            <div class="portrait-shape">
              <img :src="profilePortraitCutout" :alt="profile.name" class="portrait-img" />
            </div>
          </div>

          <!-- Bottom Stats Float Bar -->
          <div class="hero-stats-bar">
            <div class="stat-box">
              <q-icon name="article" size="18px" class="stat-icon" />
              <span class="stat-num">{{ stats.publishedPostCount ?? 0 }}</span>
              <span class="stat-label">{{ text.posts }}</span>
            </div>
            <div class="stat-sep"></div>
            <div class="stat-box">
              <q-icon name="folder_open" size="18px" class="stat-icon" />
              <span class="stat-num">{{ stats.categoryCount ?? 0 }}</span>
              <span class="stat-label">{{ text.topics }}</span>
            </div>
            <div class="stat-sep"></div>
            <div class="stat-box">
              <q-icon name="visibility" size="18px" class="stat-icon" />
              <span class="stat-num">{{ stats.totalViewCount ?? 0 }}</span>
              <span class="stat-label">{{ text.views }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue';
import type { PublicLandingResponse } from '@/types/public-landing';
import profilePortraitCutout from '@/assets/portfolio/profile-portrait-cutout-clean.png';
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';

const props = defineProps<Pick<PublicLandingResponse, 'profile' | 'stats'>>();
const { locale, text } = usePortfolioLocale();

const heroText = computed(() =>
  locale.value === 'vi'
    ? { greeting: 'Xin chào mọi người, mình là!', prefix: 'Mình là một', role: props.profile.jobTitle || 'Fullstack Web Developer' }
    : { greeting: "Hey everyone, It's Me!", prefix: "I'm a", role: props.profile.jobTitle || 'Fullstack Web Developer' }
);

const displayedRole = ref('');
let typeTimer: number | undefined;

const playTypewriter = () => {
  window.clearInterval(typeTimer);
  displayedRole.value = '';

  if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
    displayedRole.value = heroText.value.role;
    return;
  }

  let index = 0;
  typeTimer = window.setInterval(() => {
    index += 1;
    displayedRole.value = heroText.value.role.slice(0, index);
    if (index >= heroText.value.role.length) {
      window.clearInterval(typeTimer);
    }
  }, 95);
};

onMounted(playTypewriter);
onBeforeUnmount(() => window.clearInterval(typeTimer));
watch(() => heroText.value.role, playTypewriter);
</script>

<style scoped lang="scss">
.hero-section {
  padding: 64px 0 72px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.06);
}

.hero-container {
  display: grid;
  grid-template-columns: 1.15fr 0.85fr;
  gap: 48px;
  align-items: center;
}

/* Left Content */
.hero-content {
  display: flex;
  flex-direction: column;
}

.hero-eyebrow {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  font-size: 0.75rem;
  font-weight: 700;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  color: #46e0af;
  margin-bottom: 16px;
}

.status-indicator {
  width: 7px;
  height: 7px;
  border-radius: 50%;
  background: #46e0af;
  box-shadow: 0 0 8px #46e0af;
}

.eyebrow-sep {
  color: #475569;
}

.eyebrow-user {
  color: #94a3b8;
}

.hero-greeting {
  color: #f1f4ff;
  font-size: clamp(1.2rem, 2vw, 1.6rem);
  font-weight: 600;
  margin: 0 0 8px;
}

.hero-name {
  font-size: clamp(2.6rem, 5vw, 3.8rem);
  font-weight: 800;
  line-height: 1.05;
  color: #f8fafc;
  letter-spacing: -0.035em;
  margin: 0 0 12px;
}

.hero-slogan {
  color: #f1f4ff;
  font-size: clamp(1.4rem, 2.6vw, 1.95rem);
  font-weight: 700;
  line-height: 1.25;
  margin: 0 0 20px;
}

.hero-slogan-prefix {
  margin-right: 0.35em;
  display: inline-block;
}

.typewriter-text {
  color: #46e0af;
}

.typewriter-cursor {
  color: #46e0af;
  animation: blink-caret 0.75s step-end infinite;
  font-weight: 400;
}

@keyframes blink-caret {
  50% { opacity: 0; }
}

.hero-statement {
  font-size: 1.02rem;
  line-height: 1.65;
  color: #cbd5e1;
  max-width: 560px;
  margin: 0 0 24px;
}

/* Meta Strip */
.hero-meta-strip {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 20px;
  margin-bottom: 28px;
  font-size: 0.875rem;
  color: #94a3b8;
}

.meta-entry {
  display: inline-flex;
  align-items: center;
  gap: 6px;
}

.meta-icon {
  color: #46e0af;
}

.meta-link {
  color: #cbd5e1;
  text-decoration: none;
  transition: color 0.2s ease;

  &:hover {
    color: #46e0af;
  }
}

/* CTAs */
.hero-cta-group {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 14px;
  margin-bottom: 28px;
}

.cta-primary-btn {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 12px 24px;
  border-radius: 8px;
  background: #46e0af;
  color: #081126;
  font-size: 0.92rem;
  font-weight: 700;
  text-decoration: none;
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);

  &:hover {
    transform: translateY(-2px);
    box-shadow: 0 8px 20px rgba(70, 224, 175, 0.25);
  }
}

.cta-secondary-btn {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 12px 20px;
  border-radius: 8px;
  background: transparent;
  color: #e2e8f0;
  border: 1px solid rgba(255, 255, 255, 0.15);
  font-size: 0.92rem;
  font-weight: 600;
  text-decoration: none;
  transition: all 0.2s ease;

  &:hover {
    border-color: #46e0af;
    color: #46e0af;
  }
}

/* Social Strip */
.hero-social-strip {
  display: flex;
  align-items: center;
  gap: 16px;
}

.social-link-item {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-size: 0.8rem;
  font-weight: 600;
  color: #64748b;
  text-decoration: none;
  transition: color 0.2s ease;

  &:hover {
    color: #46e0af;
  }
}

/* Right Visual Column (Portrait + Shape + Float Animation) */
.hero-visual-col {
  display: flex;
  justify-content: center;
  align-items: center;
}

.portrait-panel {
  position: relative;
  width: 100%;
  max-width: 420px;
  display: flex;
  flex-direction: column;
  align-items: center;
  animation: portrait-float 4.8s ease-in-out 1s infinite;
}

@keyframes portrait-float {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-12px); }
}

@media (prefers-reduced-motion: reduce) {
  .portrait-panel {
    animation: none;
  }
}

.portrait-frame {
  position: relative;
  width: 320px;
  height: 350px;
}

.portrait-shape {
  background: #46e0af;
  border-radius: 62% 0 58% 0 / 42% 0 48% 0;
  box-shadow: 14px 18px 30px rgba(70, 224, 175, 0.18);
  height: 100%;
  overflow: hidden;
  position: relative;
  width: 100%;
}

.portrait-img {
  height: 100%;
  width: 100%;
  object-fit: cover;
  object-position: center top;
  position: absolute;
  top: 0;
  left: 50%;
  transform: translateX(-50%);
  filter: drop-shadow(10px 12px 14px rgba(3, 16, 38, 0.25));
  z-index: 1;
}

/* Bottom Stats Float Bar - Spacious, Horizontally Expanded & Beautiful */
.hero-stats-bar {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 20px;
  margin-top: 18px;
  background: rgba(15, 23, 42, 0.92);
  border: 1px solid rgba(70, 224, 175, 0.22);
  border-radius: 9999px;
  padding: 12px 28px;
  box-shadow: 0 12px 32px rgba(0, 0, 0, 0.35);
  backdrop-filter: blur(14px);
  width: max-content;
  max-width: 100%;
  white-space: nowrap;
  transition: border-color 0.3s ease, box-shadow 0.3s ease;

  &:hover {
    border-color: rgba(70, 224, 175, 0.4);
    box-shadow: 0 14px 38px rgba(70, 224, 175, 0.12);
  }
}

.stat-box {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  font-size: 0.88rem;
  color: #cbd5e1;
  white-space: nowrap;

  .stat-icon {
    color: #46e0af;
    flex-shrink: 0;
  }

  .stat-num {
    color: #46e0af;
    font-weight: 800;
    font-size: 1.08rem;
    line-height: 1;
    letter-spacing: -0.01em;
  }

  .stat-label {
    color: #cbd5e1;
    font-weight: 500;
    font-size: 0.86rem;
    white-space: nowrap;
  }
}

.stat-sep {
  width: 1px;
  height: 20px;
  background: rgba(255, 255, 255, 0.16);
  flex-shrink: 0;
}

@media (max-width: 992px) {
  .hero-container {
    grid-template-columns: 1fr;
    gap: 40px;
  }
  .hero-visual-col {
    order: -1;
  }
  .portrait-panel {
    animation: none;
    transform: none;
    max-width: 100%;
  }
  .portrait-frame {
    width: 280px;
    height: 310px;
  }
}

@media (max-width: 600px) {
  .hero-section {
    padding: 36px 0 44px;
  }
  .hero-container {
    gap: 32px;
  }
  .hero-greeting {
    font-size: 1.15rem;
    margin-bottom: 4px;
  }
  .hero-name {
    font-size: clamp(2.1rem, 8.5vw, 2.9rem);
    margin-bottom: 8px;
  }
  .hero-slogan {
    font-size: clamp(1.15rem, 5vw, 1.45rem);
    margin-bottom: 16px;
  }
  .hero-statement {
    font-size: 0.92rem;
    line-height: 1.6;
    margin-bottom: 20px;
  }
  .hero-meta-strip {
    gap: 12px;
    font-size: 0.82rem;
    margin-bottom: 22px;
  }
  .hero-cta-group {
    gap: 10px;
    margin-bottom: 22px;
  }
  .cta-primary-btn,
  .cta-secondary-btn {
    padding: 10px 18px;
    font-size: 0.86rem;
  }
  .hero-social-strip {
    gap: 14px;
    font-size: 0.76rem;
  }
  .portrait-frame {
    width: min(260px, 72vw);
    height: min(285px, 80vw);
  }
  .hero-stats-bar {
    width: 100%;
    max-width: 370px;
    box-sizing: border-box;
    padding: 9px 14px;
    gap: 8px;
    justify-content: space-around;
    margin-top: 14px;
  }
  .stat-box {
    gap: 5px;
    font-size: 0.78rem;

    .stat-icon {
      font-size: 15px;
    }
    .stat-num {
      font-size: 0.95rem;
    }
    .stat-label {
      font-size: 0.76rem;
    }
  }
  .stat-sep {
    height: 15px;
  }
}

@media (max-width: 380px) {
  .hero-stats-bar {
    padding: 8px 10px;
    gap: 4px;
  }
  .stat-box {
    gap: 4px;
    .stat-icon {
      font-size: 14px;
    }
    .stat-num {
      font-size: 0.9rem;
    }
    .stat-label {
      font-size: 0.72rem;
    }
  }
}
</style>

<!-- Global Light Mode overrides -->
<style lang="scss">
body.portfolio-light {
  .hero-section {
    border-color: #e2e8f0 !important;
  }

  .hero-greeting {
    color: #000000 !important;
  }

  .hero-name {
    color: #000000 !important;
  }

  .hero-slogan {
    color: #0f172a !important;
  }

  .typewriter-text {
    color: #0f9f74 !important;
  }

  .typewriter-cursor {
    color: #0f9f74 !important;
  }

  .hero-statement {
    color: #334155 !important;
  }

  .eyebrow-role {
    color: #0f9f74 !important;
  }

  .status-indicator {
    background: #0f9f74 !important;
    box-shadow: 0 0 8px #0f9f74 !important;
  }

  .meta-entry {
    color: #64748b !important;
  }

  .meta-icon {
    color: #0f9f74 !important;
  }

  .meta-link {
    color: #1e293b !important;
    &:hover {
      color: #0f9f74 !important;
    }
  }

  .cta-primary-btn {
    background: #0f9f74 !important;
    color: #ffffff !important;
    &:hover {
      box-shadow: 0 8px 20px rgba(15, 159, 116, 0.25) !important;
    }
  }

  .cta-secondary-btn {
    color: #1e293b !important;
    border-color: #cbd5e1 !important;
    &:hover {
      border-color: #0f9f74 !important;
      color: #0f9f74 !important;
    }
  }

  .social-link-item {
    color: #64748b !important;
    &:hover {
      color: #0f9f74 !important;
    }
  }

  .portrait-shape {
    background: #0f9f74 !important;
    box-shadow: 14px 18px 30px rgba(15, 159, 116, 0.2) !important;
  }

  .hero-stats-bar {
    background: #ffffff !important;
    border-color: #e2e8f0 !important;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08) !important;

    &:hover {
      border-color: rgba(15, 159, 116, 0.4) !important;
      box-shadow: 0 12px 34px rgba(15, 159, 116, 0.15) !important;
    }

    .stat-box {
      color: #64748b !important;
      .stat-num {
        color: #0f9f74 !important;
      }
      .stat-icon {
        color: #0f9f74 !important;
      }
      .stat-label {
        color: #475569 !important;
      }
    }

    .stat-sep {
      background: #e2e8f0 !important;
    }
  }
}
</style>
