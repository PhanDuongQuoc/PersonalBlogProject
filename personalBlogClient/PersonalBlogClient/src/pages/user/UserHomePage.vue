<template>
  <q-page class="portfolio-page">
    <main v-if="landing" class="portfolio-content">
      <portfolio-hero :profile="landing.profile" :stats="landing.stats" />

      <section id="topics" class="content-section about-section">
        <portfolio-section-title :eyebrow="text.browseByCategory" :title="text.topics" :description="text.topicsDescription" />
        <portfolio-categories :categories="landing.categories" />
      </section>

      <section id="about" class="content-section profile-section">
        <portfolio-section-title :eyebrow="text.about" :title="landing.profile.role" :description="text.profileDescription" />
        <div class="profile-details"><span>{{ text.username }}</span><strong>@{{ landing.profile.username }}</strong><span>{{ text.email }}</span><a :href="`mailto:${landing.profile.email}`">{{ landing.profile.email }}</a></div>
      </section>

      <section id="posts" class="content-section writing-section">
        <portfolio-section-title :eyebrow="text.fromTheBlog" :title="text.featuredPosts" />
        <portfolio-articles :posts="landing.featuredPosts" class="section-body" />
      </section>
    </main>

    <div v-else-if="errorMessage" class="load-state">{{ errorMessage }}</div>
    <div v-else class="load-state"><q-spinner color="deep-orange" size="32px" /> {{ text.loading }}</div>

    <portfolio-footer :email="landing?.profile.email ?? null" />
  </q-page>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';
import api from '@/boot/ApiGateway/axios';
import PortfolioArticles from '@/components/portfolio/PortfolioArticles.vue';
import PortfolioCategories from '@/components/portfolio/PortfolioCategories.vue';
import PortfolioFooter from '@/components/portfolio/PortfolioFooter.vue';
import PortfolioHero from '@/components/portfolio/PortfolioHero.vue';
import PortfolioSectionTitle from '@/components/portfolio/PortfolioSectionTitle.vue';
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';
import type { PublicLandingResponse } from '@/types/public-landing';

const landing = ref<PublicLandingResponse | null>(null);
const errorMessage = ref('');
const route = useRoute();
const { text } = usePortfolioLocale();

onMounted(async () => {
  try {
    const username = typeof route.query.username === 'string' ? route.query.username : undefined;
    const response = await api.get<PublicLandingResponse>('/public/landing', { params: { username } });
    landing.value = response.data;
  } catch {
    errorMessage.value = text.value.loadFailed;
  }
});
</script>

<style scoped lang="scss">
.portfolio-page { background: transparent; min-height: 100vh; }.portfolio-content { background: #081126; margin: 0 auto; max-width: 1060px; }.content-section { padding: 74px 78px; }.about-section { border-top: 1px solid #202b4a; }.profile-section { background: #0a1429; border-top: 1px solid #202b4a; }.writing-section { border-top: 1px solid #202b4a; }.profile-details { background: #151f37; border: 1px solid #26324e; border-radius: 2px; display: grid; gap: 11px; grid-template-columns: 130px 1fr; margin-top: 32px; max-width: 560px; padding: 22px; }.profile-details span { color: #46e0af; font-size: .7rem; font-weight: 800; letter-spacing: .1em; text-transform: uppercase; }.profile-details strong, .profile-details a { color: #e5edff; }.load-state { align-items: center; background: #081126; color: #b9cce0; display: flex; gap: 12px; justify-content: center; margin: 0 auto; max-width: 1060px; min-height: 64vh; padding: 24px; } @media (max-width: 760px) { .content-section { padding: 58px 24px; }.profile-details { grid-template-columns: 1fr; } }
</style>
