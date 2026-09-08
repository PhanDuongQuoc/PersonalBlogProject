<template>
  <section id="contact" class="about-cta-section">
    <div class="cta-card">
      <span class="cta-eyebrow">{{ text.getInTouch }}</span>
      <h2 class="cta-title">{{ text.collaborate }}</h2>
      <p class="cta-desc">{{ text.connectPrompt }}</p>

      <div class="cta-buttons">
        <a
          :href="email ? `mailto:${email}` : '#'"
          class="cta-primary-btn"
        >
          <q-icon name="mail" size="18px" />
          <span>{{ text.sayHello }}</span>
          <q-icon name="arrow_forward" size="16px" />
        </a>

        <a
          v-if="cvUrl"
          :href="cvUrl"
          target="_blank"
          rel="noopener noreferrer"
          class="cta-secondary-btn"
        >
          <q-icon name="description" size="18px" />
          <span>{{ text.downloadCv }}</span>
        </a>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';

defineProps<{
  email: string | null;
  cvUrl?: string | null;
}>();

const { text } = usePortfolioLocale();
</script>

<style scoped lang="scss">
.about-cta-section {
  border-top: 1px solid #202b4a;
  padding: 74px 78px 80px;
}

.cta-card {
  align-items: center;
  background: linear-gradient(145deg, #111e38, #0e172c);
  border: 1px solid #263859;
  border-radius: 8px;
  display: flex;
  flex-direction: column;
  padding: 56px 36px;
  text-align: center;
}

.cta-eyebrow {
  color: #46e0af;
  font-size: 0.75rem;
  font-weight: 800;
  letter-spacing: 0.14em;
  text-transform: uppercase;
}

.cta-title {
  color: #f1f4ff;
  font-size: clamp(1.8rem, 3.5vw, 2.5rem);
  font-weight: 900;
  letter-spacing: -0.04em;
  line-height: 1.15;
  margin: 14px 0 12px;
}

.cta-desc {
  color: #a0b7cf;
  font-size: 0.95rem;
  line-height: 1.6;
  margin: 0 0 32px;
  max-width: 520px;
}

.cta-buttons {
  display: flex;
  flex-wrap: wrap;
  gap: 14px;
  justify-content: center;
}

.cta-primary-btn {
  align-items: center;
  background: #46e0af;
  border-radius: 4px;
  color: #071126;
  display: inline-flex;
  font-size: 0.82rem;
  font-weight: 800;
  gap: 8px;
  padding: 12px 26px;
  text-decoration: none;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.cta-primary-btn:hover {
  box-shadow: 0 6px 20px rgba(70, 224, 175, 0.35);
  transform: translateY(-2px);
}

.cta-secondary-btn {
  align-items: center;
  background: #17243f;
  border: 1px solid #2d436c;
  border-radius: 4px;
  color: #dbe7f7;
  display: inline-flex;
  font-size: 0.82rem;
  font-weight: 700;
  gap: 8px;
  padding: 12px 24px;
  text-decoration: none;
  transition: all 0.2s ease;
}

.cta-secondary-btn:hover {
  background: #1f3156;
  border-color: #46e0af;
  color: #46e0af;
  transform: translateY(-2px);
}

@media (max-width: 760px) {
  .about-cta-section {
    padding: 58px 24px 60px;
  }

  .cta-card {
    padding: 40px 20px;
  }
}
</style>
