<template>
  <section id="top" class="hero-section">
    <div class="hero-container">
      <!-- Left Column: Editorial Positioning & Story -->
      <div class="hero-content">
        <!-- Availability / Role Eyebrow Pill -->
        <div class="hero-eyebrow">
          <span class="status-indicator"></span>
          <span class="eyebrow-role">{{ profile.role || 'FULLSTACK ENGINEER' }}</span>
          <span class="eyebrow-sep">/</span>
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
            <q-icon name="chat_bubble_outline" size="15px" />
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
            <q-icon name="code" size="15px" />
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
            <q-icon name="public" size="15px" />
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
            <q-icon name="link" size="15px" />
            <span>Website</span>
          </a>
        </div>
      </div>

      <!-- Right Column: Portrait Frame with Floating Animation & Stats -->
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
              <q-icon name="article" size="16px" class="stat-icon" />
              <span class="stat-num">{{ stats.publishedPostCount ?? 0 }}</span>
              <span class="stat-label">{{ text.posts }}</span>
            </div>
            <div class="stat-sep"></div>
            <router-link to="/topics" class="stat-box stat-box-interactive">
              <q-icon name="folder_open" size="16px" class="stat-icon" />
              <span class="stat-num">{{ stats.categoryCount ?? 0 }}</span>
              <span class="stat-label">{{ text.topics }}</span>
            </router-link>
            <div class="stat-sep"></div>
            <div class="stat-box">
              <q-icon name="visibility" size="16px" class="stat-icon" />
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
  padding: 56px 0 68px;
  border-bottom: 1px solid var(--border-hairline);
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
  font-family: var(--font-mono);
  font-size: 0.72rem;
  font-weight: 700;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  color: var(--accent-primary);
  background: var(--accent-primary-container);
  border: 1px solid rgba(16, 185, 129, 0.25);
  border-radius: var(--radius-pill);
  padding: 5px 14px;
  width: fit-content;
  margin-bottom: 18px;
}

.status-indicator {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: var(--accent-primary);
  box-shadow: 0 0 8px var(--accent-primary);
  animation: pulse-glow 2s infinite;
}

.eyebrow-sep {
  color: var(--text-muted);
}

.eyebrow-user {
  color: var(--text-secondary);
}

.hero-greeting {
  font-family: var(--font-headline);
  color: var(--text-primary);
  font-size: clamp(1.15rem, 2vw, 1.45rem);
  font-weight: 600;
  margin: 0 0 8px;
  letter-spacing: -0.01em;
}

.hero-name {
  font-family: var(--font-headline);
  font-size: clamp(2.6rem, 5.2vw, 3.8rem);
  font-weight: 800;
  line-height: 1.05;
  color: var(--text-primary);
  letter-spacing: -0.035em;
  margin: 0 0 12px;
}

.hero-slogan {
  font-family: var(--font-headline);
  color: var(--text-primary);
  font-size: clamp(1.35rem, 2.5vw, 1.85rem);
  font-weight: 700;
  line-height: 1.25;
  letter-spacing: -0.02em;
  margin: 0 0 18px;
}

.hero-slogan-prefix {
  margin-right: 0.35em;
  display: inline-block;
}

.typewriter-text {
  color: var(--accent-primary);
}

.typewriter-cursor {
  color: var(--accent-primary);
  animation: blink-caret 0.75s step-end infinite;
  font-weight: 400;
}

@keyframes blink-caret {
  50% { opacity: 0; }
}

.hero-statement {
  font-family: var(--font-body);
  font-size: 1rem;
  line-height: 1.7;
  color: var(--text-secondary);
  max-width: 560px;
  margin: 0 0 24px;
}

/* Meta Strip */
.hero-meta-strip {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 20px;
  margin-bottom: 26px;
  font-family: var(--font-body);
  font-size: 0.88rem;
  color: var(--text-secondary);
}

.meta-entry {
  display: inline-flex;
  align-items: center;
  gap: 6px;
}

.meta-icon {
  color: var(--accent-primary);
}

.meta-link {
  color: var(--text-secondary);
  text-decoration: none;
  transition: color 0.2s ease;

  &:hover {
    color: var(--accent-primary);
  }
}

/* CTAs */
.hero-cta-group {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 14px;
  margin-bottom: 26px;
}

.cta-primary-btn {
  font-family: var(--font-headline);
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 11px 22px;
  border-radius: var(--radius-md);
  background: var(--accent-primary);
  color: var(--accent-on-primary);
  font-size: 0.9rem;
  font-weight: 700;
  text-decoration: none;
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);

  &:hover {
    background: var(--accent-primary-hover);
    transform: translateY(-2px);
    box-shadow: 0 8px 24px rgba(16, 185, 129, 0.3);
  }
}

.cta-secondary-btn {
  font-family: var(--font-headline);
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 11px 20px;
  border-radius: var(--radius-md);
  background: transparent;
  color: var(--text-primary);
  border: 1px solid var(--border-subtle);
  font-size: 0.9rem;
  font-weight: 600;
  text-decoration: none;
  transition: all 0.2s ease;

  &:hover {
    border-color: var(--accent-primary);
    color: var(--accent-primary);
    background: rgba(255, 255, 255, 0.03);
  }
}

/* Social Strip */
.hero-social-strip {
  display: flex;
  align-items: center;
  gap: 16px;
}

.social-link-item {
  font-family: var(--font-mono);
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-size: 0.78rem;
  font-weight: 600;
  color: var(--text-muted);
  text-decoration: none;
  transition: color 0.2s ease;

  &:hover {
    color: var(--accent-primary);
  }
}

/* Right Visual Column */
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
  50% { transform: translateY(-10px); }
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
  background: var(--accent-primary);
  border-radius: 62% 0 58% 0 / 42% 0 48% 0;
  box-shadow: 14px 18px 30px rgba(16, 185, 129, 0.2);
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

/* Bottom Stats Float Bar */
.hero-stats-bar {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 20px;
  margin-top: 18px;
  background: var(--bg-surface-high);
  border: 1px solid var(--border-subtle);
  border-radius: var(--radius-pill);
  padding: 10px 24px;
  box-shadow: var(--shadow-card);
  backdrop-filter: blur(14px);
  width: max-content;
  max-width: 100%;
  white-space: nowrap;
  transition: all 0.3s ease;

  &:hover {
    border-color: rgba(16, 185, 129, 0.4);
    box-shadow: var(--shadow-card-hover);
  }
}

.stat-box {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  font-size: 0.86rem;
  color: var(--text-secondary);
  white-space: nowrap;
  text-decoration: none;
  transition: transform 0.2s ease;

  &.stat-box-interactive {
    cursor: pointer;

    &:hover {
      transform: translateY(-2px);

      .stat-num,
      .stat-icon {
        color: var(--accent-primary);
      }
    }
  }

  .stat-icon {
    color: var(--accent-primary);
    flex-shrink: 0;
  }

  .stat-num {
    font-family: var(--font-mono);
    color: var(--accent-primary);
    font-weight: 800;
    font-size: 1.05rem;
    line-height: 1;
  }

  .stat-label {
    color: var(--text-secondary);
    font-weight: 500;
    font-size: 0.82rem;
  }
}

.stat-sep {
  width: 1px;
  height: 18px;
  background: var(--border-hairline);
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
    gap: 28px;
  }
  .hero-name {
    font-size: clamp(2.1rem, 8.5vw, 2.9rem);
  }
  .portrait-frame {
    width: min(250px, 72vw);
    height: min(275px, 80vw);
  }
  .hero-stats-bar {
    padding: 8px 14px;
    gap: 10px;
  }
}
</style>
