<template>
  <footer id="contact" class="footer">
    <p class="footer-cta-title">{{ text.getInTouch }}</p>
    <strong class="footer-cta-desc">{{ text.collaborate }}</strong>

    <a :href="email ? `mailto:${email}` : '#'" class="footer-cta-btn">
      <span>{{ text.sayHello }}</span>
      <q-icon name="arrow_forward" size="15px" class="btn-arrow" />
    </a>

    <div class="footer-bottom-bar">
      <span class="footer-brand">PDQ Portfolio</span>
      <span class="footer-tagline">Modern Editorial Minimalist</span>
    </div>
  </footer>
</template>

<script setup lang="ts">
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';
defineProps<{ email: string | null }>();
const { text } = usePortfolioLocale();
</script>

<style scoped lang="scss">
.footer {
  align-items: center;
  border-top: 1px solid var(--border-hairline);
  color: var(--text-primary);
  display: flex;
  flex-direction: column;
  margin-top: 64px;
  padding: 80px 24px 32px;
  text-align: center;
}

.footer-cta-title {
  font-family: var(--font-headline);
  font-size: clamp(2rem, 5vw, 3.4rem);
  font-weight: 900;
  letter-spacing: -0.04em;
  margin: 0 0 14px;
  color: var(--text-primary);
}

.footer-cta-desc {
  font-family: var(--font-body);
  color: var(--text-secondary);
  font-size: 0.95rem;
  font-weight: 400;
  max-width: 520px;
  line-height: 1.6;
}

.footer-cta-btn {
  font-family: var(--font-headline);
  background: var(--accent-primary);
  color: var(--accent-on-primary);
  font-size: 0.88rem;
  font-weight: 700;
  margin-top: 28px;
  padding: 12px 28px;
  text-decoration: none;
  border-radius: var(--radius-md);
  display: inline-flex;
  align-items: center;
  gap: 8px;
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);

  .btn-arrow {
    transition: transform 0.2s ease;
  }

  &:hover {
    background: var(--accent-primary-hover);
    box-shadow: 0 8px 24px rgba(16, 185, 129, 0.3);
    transform: translateY(-2px);

    .btn-arrow {
      transform: translateX(4px);
    }
  }
}

.footer-bottom-bar {
  align-self: stretch;
  border-top: 1px solid var(--border-hairline);
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-top: 72px;
  padding-top: 24px;
  font-family: var(--font-mono);
  font-size: 0.74rem;
  color: var(--text-muted);
}

.footer-brand {
  font-weight: 700;
  color: var(--text-secondary);
}

.footer-tagline {
  font-weight: 500;
  letter-spacing: 0.04em;
}

@media (max-width: 600px) {
  .footer {
    margin-top: 40px;
    padding: 56px 16px 24px;
  }
  .footer-cta-title {
    font-size: 1.95rem;
  }
  .footer-bottom-bar {
    margin-top: 48px;
    flex-direction: column;
    gap: 8px;
    text-align: center;
  }
}
</style>
