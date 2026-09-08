<template>
  <footer id="contact" class="footer"><p>{{ text.getInTouch }}</p><strong>{{ text.collaborate }}</strong><a :href="email ? `mailto:${email}` : '#'">{{ text.sayHello }} <q-icon name="arrow_forward" /></a><small>PDQ Portfolio</small></footer>
</template>

<script setup lang="ts">
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';
defineProps<{ email: string | null }>();
const { text } = usePortfolioLocale();
</script>

<style scoped lang="scss">
.footer {
  align-items: center;
  border-top: 1px solid #202b4a;
  color: #f1f4ff;
  display: flex;
  flex-direction: column;
  margin-top: 64px;
  padding: 80px 24px 30px;
  text-align: center;
}
.footer p {
  font-size: clamp(1.8rem, 5vw, 3.2rem);
  font-weight: 900;
  letter-spacing: -.05em;
  margin: 0 0 14px;
}
.footer strong {
  color: #b3c8da;
  font-size: .88rem;
  font-weight: 400;
  max-width: 500px;
}
.footer a {
  background: #46e0af;
  color: #071126;
  font-size: .82rem;
  font-weight: 800;
  margin-top: 24px;
  padding: 12px 26px;
  text-decoration: none;
  border-radius: 6px;
  display: inline-flex;
  align-items: center;
  gap: 6px;
  transition: all 0.2s ease;

  &:hover {
    box-shadow: 0 6px 20px rgba(70, 224, 175, 0.3);
    transform: translateY(-2px);
  }
}
.footer small {
  align-self: stretch;
  border-top: 1px solid #202b4a;
  color: #8da1b9;
  font-size: .74rem;
  font-weight: 700;
  margin-top: 72px;
  padding-top: 20px;
  text-align: left;
}

@media (max-width: 600px) {
  .footer {
    margin-top: 40px;
    padding: 56px 16px 24px;
  }
  .footer p {
    font-size: 1.85rem;
  }
  .footer small {
    margin-top: 48px;
    text-align: center;
  }
}

:global(body.portfolio-light) .footer {
  background: #ffffff !important;
  border-top: 1px solid #eaedf3 !important;
  color: #0a1733 !important;

  p {
    color: #0a1733 !important;
  }
  strong {
    color: #4a5e7b !important;
  }
  small {
    border-top: 1px solid #eaedf3 !important;
    color: #718096 !important;
  }
}
</style>
