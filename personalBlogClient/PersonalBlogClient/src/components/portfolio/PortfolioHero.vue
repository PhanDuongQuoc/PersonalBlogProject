<template>
  <section id="top" class="hero">
    <div class="hero-copy">
      <p class="eyebrow">{{ profile.role }} · @{{ profile.username }}</p>
      <h1>{{ text.heroLead }}<br />{{ text.heroMiddle }}<br /><em>{{ text.heroEnd }}</em></h1>
      <p class="intro">{{ text.heroDescription.replace('{name}', profile.name) }}</p>
      <div class="hero-actions">
        <q-btn unelevated no-caps class="primary-action" href="#posts" :label="text.viewPosts" />
        <q-btn flat no-caps class="secondary-action" href="#contact" :label="text.sayHello" />
      </div>
    </div>
    <div class="portrait-panel" aria-hidden="true">
      <div class="portrait-frame">
        <div class="portrait-shape">
          <img :src="profilePortraitCutout" alt="" />
        </div>
      </div>
    </div>
    <div class="hero-stats"><span><strong>{{ stats.publishedPostCount }}</strong> {{ text.posts }}</span><span><strong>{{ stats.categoryCount }}</strong> {{ text.topics }}</span><span><strong>{{ stats.totalViewCount }}</strong> {{ text.views }}</span></div>
  </section>
</template>

<script setup lang="ts">
import type { PublicLandingResponse } from '@/types/public-landing';
import profilePortraitCutout from '@/assets/portfolio/profile-portrait-cutout-clean.png';
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';

defineProps<Pick<PublicLandingResponse, 'profile' | 'stats'>>();
const { text } = usePortfolioLocale();
</script>

<style scoped lang="scss">
.hero { display: grid; grid-template-columns: minmax(0, 1fr) 340px; min-height: 530px; padding: 112px 42px 72px; position: relative; }.hero-copy { position: relative; z-index: 1; }.eyebrow { color: #46e0af; font-size: .75rem; font-weight: 800; letter-spacing: .1em; text-transform: uppercase; }h1 { color: #f1f4ff; font-size: clamp(3.25rem, 6vw, 5.25rem); font-weight: 900; letter-spacing: -.065em; line-height: 1.02; margin: 22px 0 24px; }h1 em { color: #d7e3ff; font-style: normal; }.intro { color: #b9cce0; font-size: .93rem; line-height: 1.65; max-width: 590px; }.hero-actions { display: flex; gap: 10px; margin-top: 30px; }.primary-action { background: #46e0af; border-radius: 2px; color: #071126; font-size: .75rem; font-weight: 800; min-width: 102px; }.secondary-action { border: 1px solid #2ea990; border-radius: 2px; color: #57efd0; font-size: .75rem; }.portrait-panel { align-self: end; height: 410px; position: relative; width: 340px; }.portrait-frame { animation: frame-enter .7s ease-out both; bottom: 0; height: 365px; position: absolute; right: 0; width: 330px; }.portrait-shape { background: #46e0af; border-radius: 62% 0 58% 0 / 42% 0 48% 0; box-shadow: 14px 18px 30px rgba(70, 224, 175, .18); height: 100%; overflow: hidden; position: relative; width: 100%; }.portrait-panel img { animation: portrait-enter .8s cubic-bezier(.2, .8, .2, 1) .12s both; filter: drop-shadow(10px 12px 12px rgba(3, 16, 38, .18)); height: 100%; left: 50%; object-fit: cover; object-position: center top; position: absolute; top: 0; transform: translateX(-50%); width: 100%; z-index: 1; }.hero-stats { bottom: 38px; color: #a9c1d3; display: flex; gap: 28px; position: absolute; right: 42px; z-index: 3; }.hero-stats strong { color: #46e0af; margin-right: 4px; }@keyframes frame-enter { from { opacity: 0; transform: scale(.94) translateY(16px); } to { opacity: 1; transform: scale(1) translateY(0); } }@keyframes portrait-enter { from { opacity: 0; transform: translateX(-50%) translateY(18px); } to { opacity: 1; transform: translateX(-50%) translateY(0); } }@media (prefers-reduced-motion: reduce) { .portrait-frame, .portrait-panel img { animation: none; } }@media (max-width: 760px) { .hero { display: block; min-height: 650px; padding: 100px 24px 120px; }.portrait-panel { bottom: 58px; height: 260px; opacity: .78; position: absolute; right: 18px; width: 210px; }.portrait-frame { bottom: 0; height: 230px; right: 0; width: 208px; }.portrait-shape { height: 100%; width: 100%; }.portrait-panel img { transform: translateX(-50%); }.hero-stats { bottom: 42px; flex-wrap: wrap; left: 24px; right: auto; } }

/* A calmer display treatment keeps the hero readable on narrower screens. */
h1 {
  font-size: clamp(2.85rem, 5.2vw, 4.7rem);
  font-weight: 800;
  letter-spacing: -0.045em;
  line-height: 1.1;
  max-width: 670px;
}

.intro {
  font-size: 1rem;
  line-height: 1.7;
}
</style>
