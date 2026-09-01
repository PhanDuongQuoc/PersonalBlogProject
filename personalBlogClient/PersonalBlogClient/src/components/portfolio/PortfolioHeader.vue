<template>
  <q-header class="portfolio-header">
    <q-toolbar class="portfolio-toolbar">
      <a class="brand" href="#top" :aria-label="text.goTop">{{ brand }}<span>Portfolio</span></a>
      <div class="desktop-nav">
        <a v-for="item in navigation" :key="item.target" :href="item.target" :class="{ active: activeTarget === item.target }" @click="activeTarget = item.target">{{ item.label }}</a>
      </div>
      <q-btn-toggle v-model="locale" unelevated no-caps dense class="language-toggle" :options="[{ label: 'VI', value: 'vi' }, { label: 'EN', value: 'en' }]" @update:model-value="setLocale" />
      <q-btn flat round dense class="theme-button" :icon="isDark ? 'light_mode' : 'dark_mode'" :aria-label="isDark ? text.lightMode : text.darkMode" @click="toggleTheme" />
      <q-btn unelevated no-caps class="contact-button" href="#contact" :label="text.resume" />
      <q-btn flat round dense class="mobile-menu-button" icon="menu" aria-label="Open navigation menu" @click="mobileMenuOpen = true" />
    </q-toolbar>

    <q-dialog v-model="mobileMenuOpen" position="left" full-height>
      <aside class="mobile-drawer">
        <div class="mobile-drawer-top">
          <a class="mobile-brand" href="#top" @click="mobileMenuOpen = false">{{ brand }}<span>Portfolio</span></a>
          <q-btn flat round dense icon="close" aria-label="Close navigation menu" @click="mobileMenuOpen = false" />
        </div>
        <nav class="mobile-navigation">
          <a v-for="item in navigation" :key="item.target" :href="item.target" :class="{ active: activeTarget === item.target }" @click.prevent="navigate(item.target)">
            <q-icon :name="item.icon" size="20px" />
            <span>{{ item.label }}</span>
          </a>
        </nav>
        <div class="mobile-drawer-footer">
          <div class="drawer-setting"><span>{{ locale === 'vi' ? 'Ngôn ngữ' : 'Language' }}</span><q-btn-toggle v-model="locale" unelevated no-caps dense :options="[{ label: 'VI', value: 'vi' }, { label: 'EN', value: 'en' }]" @update:model-value="setLocale" /></div>
          <button class="drawer-setting drawer-theme" type="button" @click="toggleTheme"><span>{{ isDark ? text.lightMode : text.darkMode }}</span><q-icon :name="isDark ? 'light_mode' : 'dark_mode'" size="20px" /></button>
          <a class="drawer-resume" href="#contact" @click.prevent="navigate('#contact')">{{ text.resume }} <q-icon name="arrow_forward" /></a>
        </div>
      </aside>
    </q-dialog>
  </q-header>
</template>

<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref } from 'vue';
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';

const { locale, setLocale, text } = usePortfolioLocale();
const navigation = computed(() => [
  { label: text.value.home, target: '#top', icon: 'home' }, { label: text.value.about, target: '#about', icon: 'person_outline' }, { label: text.value.topics, target: '#topics', icon: 'folder_open' }, { label: text.value.posts, target: '#posts', icon: 'article' }, { label: text.value.contact, target: '#contact', icon: 'mail_outline' }
]);
const brand = 'PDQ';
const isDark = ref(true);
const activeTarget = ref('#top');
const mobileMenuOpen = ref(false);

const navigate = (target: string) => {
  activeTarget.value = target;
  mobileMenuOpen.value = false;
  window.history.replaceState(null, '', `${window.location.pathname}${window.location.search}${target}`);
  document.querySelector(target)?.scrollIntoView({ behavior: 'smooth' });
};

const applyTheme = () => {
  document.body.classList.toggle('portfolio-light', !isDark.value);
  localStorage.setItem('portfolio-theme', isDark.value ? 'dark' : 'light');
};

const toggleTheme = () => {
  isDark.value = !isDark.value;
  applyTheme();
};

onMounted(() => {
  isDark.value = localStorage.getItem('portfolio-theme') !== 'light';
  applyTheme();
  window.addEventListener('scroll', updateActiveSection, { passive: true });
  updateActiveSection();
});

onBeforeUnmount(() => {
  window.removeEventListener('scroll', updateActiveSection);
});

const updateActiveSection = () => {
  const current = [...navigation.value]
    .reverse()
    .find((item) => {
      const section = document.querySelector(item.target);
      return section !== null && section.getBoundingClientRect().top <= 150;
    });

  activeTarget.value = current?.target ?? '#top';
};
</script>

<style scoped lang="scss">
.portfolio-header { background: #081126; border-bottom: 1px solid #202b4a; color: #f2f5ff; }
.portfolio-toolbar { max-width: 1060px; min-height: 76px; margin: auto; padding: 0 42px; }
.brand { color: #46e0af; font-size: 1.45rem; font-weight: 900; letter-spacing: -.06em; text-decoration: none; }
.brand span { color: #f2f5ff; }
.desktop-nav { display: flex; gap: 30px; margin-left: auto; margin-right: 44px; }
.desktop-nav a { color: #b9c6d9; font-size: .79rem; font-weight: 600; padding: 28px 0 25px; position: relative; text-decoration: none; }
.desktop-nav a:hover, .desktop-nav a.active { color: #46e0af; }.desktop-nav a.active::after { background: #46e0af; bottom: 17px; content: ''; height: 2px; left: 0; position: absolute; right: 0; }
.language-toggle { border: 1px solid #2a4960; border-radius: 2px; color: #b9cce0; font-size: .68rem; margin-right: 12px; }.language-toggle :deep(.q-btn) { min-height: 28px; padding: 0 8px; }.language-toggle :deep(.q-btn--active), .mobile-drawer :deep(.q-btn--active) { background: #46e0af; color: #071126; }.theme-button { color: #b9cce0; margin-right: 12px; }.contact-button { background: #46e0af; border-radius: 2px; color: #071126; font-size: .73rem; font-weight: 800; min-width: 92px; }.mobile-menu-button { display: none; }
.mobile-drawer { background: linear-gradient(180deg, #151f3b, #0d162c); border-right: 1px solid #2c3f68; box-shadow: 18px 0 40px rgba(0, 0, 0, .28); color: #eff4ff; display: flex; flex-direction: column; height: 100vh; padding: 26px 18px; width: min(82vw, 310px); }.mobile-drawer-top { align-items: center; display: flex; justify-content: space-between; padding: 4px 6px 28px; }.mobile-brand { color: #46e0af; font-size: 1.28rem; font-weight: 800; letter-spacing: -.04em; text-decoration: none; }.mobile-brand span { color: #f1f4ff; }.mobile-drawer-top .q-btn { color: #aab9d4; }.mobile-navigation { display: grid; gap: 7px; }.mobile-navigation a { align-items: center; border-left: 3px solid transparent; color: #aebcdb; display: flex; font-size: .95rem; font-weight: 600; gap: 17px; min-height: 48px; padding: 0 14px; text-decoration: none; }.mobile-navigation a.active, .mobile-navigation a:hover { background: rgba(70, 224, 175, .09); border-left-color: #46e0af; color: #f2f6ff; }.mobile-drawer-footer { border-top: 1px solid #2a3b60; display: grid; gap: 15px; margin-top: auto; padding: 22px 8px 4px; }.drawer-setting { align-items: center; color: #b8c6df; display: flex; font-size: .82rem; justify-content: space-between; }.drawer-theme { background: none; border: 0; cursor: pointer; font-family: inherit; padding: 0; text-align: left; }.drawer-theme .q-icon { color: #46e0af; }.drawer-resume { align-items: center; background: #46e0af; color: #071126; display: flex; font-size: .78rem; font-weight: 800; justify-content: space-between; margin-top: 6px; padding: 12px 14px; text-decoration: none; }
@media (max-width: 600px) { .portfolio-toolbar { padding: 0 16px; } .desktop-nav, .language-toggle, .theme-button, .contact-button { display: none; } .mobile-menu-button { color: #46e0af; display: inline-flex; margin-left: auto; } }
</style>
