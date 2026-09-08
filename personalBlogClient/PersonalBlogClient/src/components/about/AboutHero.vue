<template>
  <section class="about-hero">
    <div class="hero-left">
      <div class="eyebrow-tag">
        <span class="pulse-dot"></span>
        {{ profile.role }} · @{{ profile.username }}
      </div>

      <h1 class="hero-name">{{ profile.name }}</h1>

      <p class="hero-job-title">
        <span class="accent-text">{{ profile.jobTitle || 'Fullstack Web Developer' }}</span>
      </p>

      <p class="hero-bio">
        {{ profile.bio || text.aboutPageBio }}
      </p>

      <div class="meta-row">
        <div v-if="profile.location" class="meta-item">
          <q-icon name="place" size="18px" />
          <span>{{ profile.location }}</span>
        </div>
        <div class="meta-item">
          <q-icon name="email" size="18px" />
          <a :href="`mailto:${profile.email}`">{{ profile.email }}</a>
        </div>
        <div v-if="profile.phone" class="meta-item">
          <q-icon name="phone" size="18px" />
          <span>{{ profile.phone }}</span>
        </div>
      </div>

      <div class="hero-actions">
        <q-btn
          v-if="profile.cvUrl"
          unelevated
          no-caps
          class="primary-action"
          :href="profile.cvUrl"
          target="_blank"
          icon="description"
          :label="text.downloadCv"
        />
        <q-btn
          flat
          no-caps
          class="secondary-action"
          href="#contact"
          icon="chat_bubble_outline"
          :label="text.sayHello"
        />
      </div>

      <!-- Social Links -->
      <div class="social-links">
        <a
          v-if="profile.githubUrl"
          :href="profile.githubUrl"
          target="_blank"
          rel="noopener noreferrer"
          class="social-btn"
          aria-label="GitHub"
        >
          <q-icon name="code" size="20px" />
          <span>GitHub</span>
        </a>
        <a
          v-if="profile.linkedinUrl"
          :href="profile.linkedinUrl"
          target="_blank"
          rel="noopener noreferrer"
          class="social-btn"
          aria-label="LinkedIn"
        >
          <q-icon name="public" size="20px" />
          <span>LinkedIn</span>
        </a>
        <a
          v-if="profile.websiteUrl"
          :href="profile.websiteUrl"
          target="_blank"
          rel="noopener noreferrer"
          class="social-btn"
          aria-label="Website"
        >
          <q-icon name="language" size="20px" />
          <span>Website</span>
        </a>
        <a
          v-if="profile.facebookUrl"
          :href="profile.facebookUrl"
          target="_blank"
          rel="noopener noreferrer"
          class="social-btn"
          aria-label="Facebook"
        >
          <q-icon name="share" size="20px" />
          <span>Facebook</span>
        </a>
      </div>
    </div>

    <div class="hero-right">
      <div class="portrait-container">
        <div class="portrait-shape-wrapper">
          <div class="portrait-backdrop"></div>
          <img
            v-if="profile.avatarUrl"
            :src="profile.avatarUrl"
            :alt="profile.name"
            class="portrait-img"
          />
          <img
            v-else
            :src="defaultPortrait"
            alt="Profile portrait"
            class="portrait-img"
          />
        </div>
        <div class="exp-badge">
          <strong>{{ profile.yearsOfExperience }}+</strong>
          <span>{{ text.yearsExpLabel }}</span>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import defaultPortrait from '@/assets/portfolio/profile-portrait-cutout-clean.png';
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';
import type { PublicAboutProfile } from '@/types/public-about';

defineProps<{ profile: PublicAboutProfile }>();
const { text } = usePortfolioLocale();
</script>

<style scoped lang="scss">
.about-hero {
  display: grid;
  grid-template-columns: minmax(0, 1fr) 340px;
  gap: 40px;
  min-height: 480px;
  padding: 80px 42px 60px;
  position: relative;
  align-items: center;
}

.hero-left {
  position: relative;
  z-index: 1;
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

.hero-name {
  color: #f1f4ff;
  font-size: clamp(2.6rem, 4.5vw, 3.8rem);
  font-weight: 900;
  letter-spacing: -0.05em;
  line-height: 1.1;
  margin: 18px 0 10px;
}

.hero-job-title {
  color: #dce8ff;
  font-size: clamp(1.25rem, 2.2vw, 1.6rem);
  font-weight: 700;
  margin: 0 0 20px;
}

.accent-text {
  color: #46e0af;
}

.hero-bio {
  color: #b9cce0;
  font-size: 1rem;
  line-height: 1.7;
  margin: 0 0 26px;
  max-width: 580px;
}

.meta-row {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  column-gap: 16px;
  row-gap: 8px;
  margin-bottom: 28px;
}

.meta-item {
  align-items: center;
  color: #a0b6cc;
  display: inline-flex;
  font-size: 0.81rem;
  gap: 6px;
  white-space: nowrap;
}

.meta-item .q-icon {
  color: #35d6ff;
}

.meta-item a {
  color: #e2ecfa;
  text-decoration: none;
  transition: color 0.2s ease;
}

.meta-item a:hover {
  color: #46e0af;
}

.hero-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  margin-bottom: 28px;
}

.primary-action {
  background: #46e0af;
  border-radius: 3px;
  color: #071126;
  font-size: 0.8rem;
  font-weight: 800;
  letter-spacing: 0.02em;
  padding: 10px 20px;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.primary-action:hover {
  box-shadow: 0 6px 20px rgba(70, 224, 175, 0.35);
  transform: translateY(-2px);
}

.secondary-action {
  border: 1px solid #2ea990;
  border-radius: 3px;
  color: #57efd0;
  font-size: 0.8rem;
  font-weight: 700;
  padding: 10px 20px;
}

.secondary-action:hover {
  background: rgba(70, 224, 175, 0.08);
}

.social-links {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

.social-btn {
  align-items: center;
  background: #151f37;
  border: 1px solid #26324e;
  border-radius: 3px;
  color: #aebcdb;
  display: inline-flex;
  font-size: 0.78rem;
  font-weight: 600;
  gap: 6px;
  padding: 7px 13px;
  text-decoration: none;
  transition: all 0.2s ease;
}

.social-btn:hover {
  background: #1c2b4d;
  border-color: #46e0af;
  color: #46e0af;
  transform: translateY(-2px);
}

.hero-right {
  display: flex;
  justify-content: center;
  position: relative;
}

.portrait-container {
  height: 380px;
  position: relative;
  width: 310px;
  animation: float-portrait 4.8s ease-in-out infinite;
}

@keyframes float-portrait {
  0%, 100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-14px);
  }
}

.portrait-shape-wrapper {
  background: linear-gradient(145deg, #12213d, #0d172e);
  border: 1px solid #263859;
  border-radius: 24px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.35);
  height: 100%;
  overflow: hidden;
  position: relative;
  width: 100%;
}

.portrait-backdrop {
  background: radial-gradient(circle at 50% 30%, rgba(70, 224, 175, 0.25) 0%, transparent 70%);
  height: 100%;
  left: 0;
  position: absolute;
  top: 0;
  width: 100%;
}

.portrait-img {
  bottom: 0;
  filter: drop-shadow(0 12px 16px rgba(0, 0, 0, 0.4));
  height: 94%;
  left: 50%;
  object-fit: cover;
  object-position: center top;
  position: absolute;
  transform: translateX(-50%);
  width: 90%;
  z-index: 1;
}

.exp-badge {
  align-items: center;
  background: #151f37;
  border: 1px solid #2c416a;
  border-radius: 8px;
  bottom: 12px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.35);
  display: flex;
  flex-direction: column;
  left: -20px;
  padding: 10px 16px;
  position: absolute;
  z-index: 2;
  animation: float-badge 4.8s ease-in-out infinite;
  animation-delay: 0.3s;
}

@keyframes float-badge {
  0%, 100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-6px);
  }
}

.exp-badge strong {
  color: #46e0af;
  font-size: 1.4rem;
  font-weight: 900;
  line-height: 1;
}

.exp-badge span {
  color: #b9cce0;
  font-size: 0.68rem;
  font-weight: 700;
  letter-spacing: 0.04em;
  margin-top: 3px;
  text-transform: uppercase;
}

@media (max-width: 760px) {
  .about-hero {
    grid-template-columns: 1fr;
    padding: 60px 24px 40px;
  }

  .hero-right {
    margin-top: 20px;
    order: -1;
  }

  .portrait-container {
    height: 300px;
    width: 250px;
  }

  .exp-badge {
    left: 0;
  }
}

:global(body.portfolio-light) .about-hero {
  .hero-name {
    color: #0a1733;
  }
  .hero-job-title {
    color: #0f9f74;
  }
  .hero-bio {
    color: #354764;
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
  .meta-item {
    color: #4a5e7b;
    .q-icon {
      color: #0b8062;
    }
    a {
      color: #1c2e4f;
      &:hover {
        color: #0f9f74;
      }
    }
  }
  .social-btn {
    background: #ffffff;
    border-color: #ccd7e6;
    color: #263857;
    &:hover {
      background: #f0f6ff;
      border-color: #0f9f74;
      color: #0f9f74;
    }
  }
  .exp-badge {
    background: #ffffff;
    border-color: #ccd7e6;
    box-shadow: 0 10px 25px rgba(21, 33, 60, 0.12);
    strong {
      color: #0f9f74;
    }
    span {
      color: #4a5e7b;
    }
  }
}
</style>
