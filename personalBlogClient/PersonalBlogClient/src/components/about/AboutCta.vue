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
          <q-icon name="arrow_forward" size="16px" class="btn-arrow" />
        </a>

        <a
          v-if="cvUrl"
          :href="cvUrl"
          target="_blank"
          rel="noopener noreferrer"
          class="cta-secondary-btn"
        >
          <q-icon name="description" size="17px" />
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
  padding: 64px 0 80px;
}

.cta-card {
  align-items: center;
  background: var(--bg-surface-low);
  border: 1px solid var(--border-hairline);
  border-radius: var(--radius-xl);
  display: flex;
  flex-direction: column;
  padding: 56px 36px;
  text-align: center;
  box-shadow: var(--shadow-card);
}

.cta-eyebrow {
  font-family: var(--font-mono);
  color: var(--accent-primary);
  font-size: 0.74rem;
  font-weight: 800;
  letter-spacing: 0.12em;
  text-transform: uppercase;
}

.cta-title {
  font-family: var(--font-headline);
  color: var(--text-primary);
  font-size: clamp(1.8rem, 3.5vw, 2.5rem);
  font-weight: 900;
  letter-spacing: -0.035em;
  line-height: 1.15;
  margin: 12px 0 10px;
}

.cta-desc {
  font-family: var(--font-body);
  color: var(--text-secondary);
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
  font-family: var(--font-headline);
  align-items: center;
  background: var(--accent-primary);
  border-radius: var(--radius-md);
  color: var(--accent-on-primary);
  display: inline-flex;
  font-size: 0.88rem;
  font-weight: 700;
  gap: 8px;
  padding: 11px 26px;
  text-decoration: none;
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);

  .btn-arrow {
    transition: transform 0.2s ease;
  }

  &:hover {
    background: var(--accent-primary-hover);
    box-shadow: 0 6px 20px rgba(16, 185, 129, 0.3);
    transform: translateY(-2px);

    .btn-arrow {
      transform: translateX(4px);
    }
  }
}

.cta-secondary-btn {
  font-family: var(--font-headline);
  align-items: center;
  background: transparent;
  border: 1px solid var(--border-subtle);
  border-radius: var(--radius-md);
  color: var(--text-primary);
  display: inline-flex;
  font-size: 0.88rem;
  font-weight: 600;
  gap: 8px;
  padding: 11px 22px;
  text-decoration: none;
  transition: all 0.2s ease;

  &:hover {
    border-color: var(--accent-primary);
    color: var(--accent-primary);
    background: rgba(255, 255, 255, 0.03);
    transform: translateY(-2px);
  }
}

@media (max-width: 760px) {
  .cta-card {
    padding: 40px 20px;
  }
}
</style>
