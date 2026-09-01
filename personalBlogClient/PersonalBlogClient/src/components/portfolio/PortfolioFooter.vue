<template>
  <footer id="contact" class="footer"><p>{{ text.getInTouch }}</p><strong>{{ text.collaborate }}</strong><a :href="email ? `mailto:${email}` : '#'">{{ text.sayHello }} <q-icon name="arrow_forward" /></a><small>PDQ Portfolio</small></footer>
</template>

<script setup lang="ts">
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';
defineProps<{ email: string | null }>();
const { text } = usePortfolioLocale();
</script>

<style scoped lang="scss">
.footer { align-items: center; border-top: 1px solid #202b4a; color: #f1f4ff; display: flex; flex-direction: column; margin-top: 76px; padding: 98px 24px 34px; text-align: center; }.footer p { font-size: clamp(2rem, 4vw, 3.2rem); font-weight: 900; letter-spacing: -.06em; margin: 0 0 16px; }.footer strong { color: #b3c8da; font-size: .87rem; font-weight: 400; }.footer a { background: #46e0af; color: #071126; font-size: .76rem; font-weight: 800; margin-top: 29px; padding: 12px 26px; text-decoration: none; }.footer small { align-self: stretch; border-top: 1px solid #202b4a; color: #bed0e5; font-size: .74rem; font-weight: 800; margin-top: 110px; padding-top: 25px; text-align: left; }
</style>
