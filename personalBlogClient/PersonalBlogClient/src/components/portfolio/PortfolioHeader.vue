<template>
  <q-header class="portfolio-header">
    <q-toolbar class="portfolio-toolbar">
      <!-- Brand / Logo -->
      <router-link class="brand" to="/home" :aria-label="text.goTop">
        {{ brand }}<span class="brand-suffix">Portfolio</span>
      </router-link>

      <!-- Desktop Navigation -->
      <nav class="desktop-nav">
        <a
          v-for="item in navigation"
          :key="item.key"
          :href="item.target"
          :class="{ active: isItemActive(item) }"
          @click.prevent="handleNavClick(item)"
        >
          {{ item.label }}
        </a>
      </nav>

      <!-- Language Switcher -->
      <q-btn-toggle
        v-model="locale"
        unelevated
        no-caps
        dense
        class="language-toggle"
        :options="[{ label: 'VI', value: 'vi' }, { label: 'EN', value: 'en' }]"
        @update:model-value="setLocale"
      />

      <!-- Theme Switcher -->
      <q-btn
        flat
        round
        dense
        class="theme-button"
        :icon="isDark ? 'light_mode' : 'dark_mode'"
        :aria-label="isDark ? text.lightMode : text.darkMode"
        @click="toggleTheme"
      />

      <!-- Action CTA Button -->
      <a
        href="#contact"
        class="header-cta-btn"
        @click.prevent="scrollToContact"
      >
        <span>{{ text.resume }}</span>
        <q-icon name="arrow_forward" size="14px" />
      </a>

      <!-- Mobile Hamburger Button -->
      <q-btn
        flat
        round
        dense
        class="mobile-menu-button"
        icon="menu"
        aria-label="Open navigation menu"
        @click="mobileMenuOpen = true"
      />
    </q-toolbar>

    <!-- Mobile Slide Drawer -->
    <q-dialog v-model="mobileMenuOpen" position="left" full-height persistent>
      <aside class="mobile-drawer" @click.stop @mousedown.stop @touchstart.stop>
        <div class="mobile-drawer-top">
          <router-link class="mobile-brand" to="/home" @click="mobileMenuOpen = false">
            {{ brand }}<span>Portfolio</span>
          </router-link>
          <q-btn flat round dense icon="close" aria-label="Close menu" @click="mobileMenuOpen = false" />
        </div>

        <nav class="mobile-navigation">
          <a
            v-for="item in navigation"
            :key="item.key"
            :href="item.target"
            :class="{ active: isItemActive(item) }"
            @click.stop.prevent="handleNavClick(item)"
          >
            <q-icon :name="item.icon" size="20px" class="nav-ico" />
            <span>{{ item.label }}</span>
          </a>
        </nav>

        <div class="mobile-drawer-footer">
          <div class="drawer-setting">
            <span>{{ locale === 'vi' ? 'Ngôn ngữ' : 'Language' }}</span>
            <q-btn-toggle
              v-model="locale"
              unelevated
              no-caps
              dense
              class="drawer-lang-toggle"
              :options="[{ label: 'VI', value: 'vi' }, { label: 'EN', value: 'en' }]"
              @update:model-value="setLocale"
            />
          </div>

          <button class="drawer-setting drawer-theme" type="button" @click="toggleTheme">
            <span>{{ isDark ? text.lightMode : text.darkMode }}</span>
            <q-icon :name="isDark ? 'light_mode' : 'dark_mode'" size="20px" class="theme-ico" />
          </button>

          <a class="drawer-resume" href="#contact" @click.stop.prevent="scrollToContact">
            <span>{{ text.resume }}</span>
            <q-icon name="arrow_forward" size="16px" />
          </a>
        </div>
      </aside>
    </q-dialog>
  </q-header>
</template>

<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';

const { locale, setLocale, text } = usePortfolioLocale();
const route = useRoute();
const router = useRouter();

interface NavItem {
  key: string;
  label: string;
  target: string;
  icon: string;
  route?: string;
}

const navigation = computed<NavItem[]>(() => [
  { key: 'home', label: text.value.home, target: '/home', icon: 'home', route: '/home' },
  { key: 'about', label: text.value.about, target: '/about', icon: 'person_outline', route: '/about' },
  { key: 'topics', label: text.value.topics, target: '/topics', icon: 'folder_open', route: '/topics' },
  { key: 'contact', label: text.value.contact, target: '#contact', icon: 'mail_outline' }
]);

const brand = 'PDQ';
const isDark = ref(true);
const mobileMenuOpen = ref(false);

const isItemActive = (item: NavItem) => {
  if (item.key === 'home') {
    return route.path === '/home' || route.path === '/';
  }
  if (item.key === 'about') {
    return route.path === '/about';
  }
  if (item.key === 'topics') {
    return route.path.startsWith('/topics');
  }
  return false;
};

const handleNavClick = async (item: NavItem) => {
  mobileMenuOpen.value = false;

  if (item.key === 'about') {
    if (route.path !== '/about') {
      await router.push({ path: '/about', query: route.query });
    }
    window.scrollTo({ top: 0, behavior: 'smooth' });
    return;
  }

  if (item.key === 'topics') {
    if (route.path !== '/topics') {
      await router.push({ path: '/topics', query: route.query });
    }
    window.scrollTo({ top: 0, behavior: 'smooth' });
    return;
  }

  if (item.key === 'home') {
    if (route.path !== '/home') {
      await router.push({ path: '/home', query: route.query });
    }
    window.scrollTo({ top: 0, behavior: 'smooth' });
    return;
  }

  // Anchor targets (#posts, #contact)
  if (route.path !== '/home') {
    await router.push({ path: '/home', query: route.query, hash: item.target });
    setTimeout(() => {
      document.querySelector(item.target)?.scrollIntoView({ behavior: 'smooth' });
    }, 250);
  } else {
    document.querySelector(item.target)?.scrollIntoView({ behavior: 'smooth' });
  }
};

const scrollToContact = () => {
  mobileMenuOpen.value = false;
  const contactEl = document.querySelector('#contact');
  if (contactEl) {
    contactEl.scrollIntoView({ behavior: 'smooth' });
  }
};

const applyTheme = () => {
  document.body.classList.toggle('portfolio-light', !isDark.value);
  localStorage.setItem('portfolio-theme', isDark.value ? 'dark' : 'light');
};

const toggleTheme = () => {
  isDark.value = !isDark.value;
  applyTheme();
};

const closeMobileMenuOnDesktop = () => {
  if (window.innerWidth > 860) {
    mobileMenuOpen.value = false;
  }
};

onMounted(() => {
  isDark.value = localStorage.getItem('portfolio-theme') !== 'light';
  applyTheme();
  window.addEventListener('resize', closeMobileMenuOnDesktop, { passive: true });
  closeMobileMenuOnDesktop();
});

onBeforeUnmount(() => {
  window.removeEventListener('resize', closeMobileMenuOnDesktop);
});
</script>

<style scoped lang="scss">
.portfolio-header {
  background: var(--glass-header-bg);
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  border-bottom: 1px solid var(--glass-header-border);
  position: sticky;
  top: 0;
  z-index: 1000;
  transition: all 0.3s ease;
}

.portfolio-toolbar {
  max-width: 1140px;
  min-height: 72px;
  margin: 0 auto;
  padding: 0 32px;
}

/* Brand */
.brand {
  font-family: var(--font-headline);
  color: var(--accent-primary);
  font-size: 1.4rem;
  font-weight: 800;
  letter-spacing: -0.04em;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  gap: 4px;
}

.brand-suffix {
  color: var(--text-primary);
  font-weight: 600;
  font-size: 1.3rem;
  letter-spacing: -0.03em;
}

/* Desktop Navigation */
.desktop-nav {
  display: flex;
  align-items: center;
  gap: 32px;
  margin-left: auto;
  margin-right: 28px;
}

.desktop-nav a {
  font-family: var(--font-headline);
  color: var(--text-secondary);
  font-size: 0.88rem;
  font-weight: 600;
  text-decoration: none;
  padding: 24px 0;
  position: relative;
  transition: color 0.2s ease;

  &:hover {
    color: var(--accent-primary);
  }

  &.active {
    color: var(--accent-primary);

    &::after {
      content: '';
      position: absolute;
      bottom: 18px;
      left: 0;
      right: 0;
      height: 2px;
      background: var(--accent-primary);
      border-radius: 2px;
    }
  }
}

/* Controls */
.language-toggle {
  border: 1px solid var(--border-subtle);
  border-radius: var(--radius-sm);
  margin-right: 12px;
  font-family: var(--font-mono);
  font-size: 0.72rem;
  font-weight: 700;

  :deep(.q-btn) {
    min-height: 28px;
    padding: 0 8px;
    color: var(--text-secondary);
  }

  :deep(.q-btn--active) {
    background: var(--accent-primary);
    color: var(--accent-on-primary);
    font-weight: 800;
  }
}

.theme-button {
  color: var(--text-secondary);
  margin-right: 16px;
  transition: color 0.2s ease, transform 0.2s ease;

  &:hover {
    color: var(--accent-primary);
    transform: rotate(15deg);
  }
}

.header-cta-btn {
  font-family: var(--font-headline);
  font-size: 0.84rem;
  font-weight: 700;
  background: var(--accent-primary);
  color: var(--accent-on-primary);
  border-radius: var(--radius-md);
  padding: 8px 18px;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  gap: 6px;
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);

  &:hover {
    background: var(--accent-primary-hover);
    transform: translateY(-2px);
    box-shadow: 0 6px 20px rgba(16, 185, 129, 0.3);
  }
}

.mobile-menu-button {
  display: none;
}

/* Mobile Drawer */
.mobile-drawer {
  background: var(--bg-surface-high);
  border-right: 1px solid var(--border-hairline);
  box-shadow: 18px 0 40px rgba(0, 0, 0, 0.35);
  color: var(--text-primary);
  display: flex;
  flex-direction: column;
  height: 100vh;
  padding: 24px 20px;
  width: min(82vw, 320px);
  pointer-events: auto;
}

.mobile-drawer-top {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding-bottom: 24px;
  border-bottom: 1px solid var(--border-hairline);
}

.mobile-brand {
  font-family: var(--font-headline);
  color: var(--accent-primary);
  font-size: 1.3rem;
  font-weight: 800;
  text-decoration: none;

  span {
    color: var(--text-primary);
  }
}

.mobile-navigation {
  display: flex;
  flex-direction: column;
  gap: 6px;
  margin-top: 18px;
}

.mobile-navigation a {
  display: flex;
  align-items: center;
  gap: 14px;
  padding: 12px 16px;
  border-radius: var(--radius-md);
  font-family: var(--font-headline);
  font-size: 0.95rem;
  font-weight: 600;
  color: var(--text-secondary);
  text-decoration: none;
  transition: all 0.2s ease;

  .nav-ico {
    color: var(--text-muted);
    transition: color 0.2s ease;
  }

  &:hover,
  &.active {
    background: var(--accent-primary-container);
    color: var(--accent-primary);

    .nav-ico {
      color: var(--accent-primary);
    }
  }
}

.mobile-drawer-footer {
  margin-top: auto;
  border-top: 1px solid var(--border-hairline);
  padding-top: 20px;
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.drawer-setting {
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 0.85rem;
  color: var(--text-secondary);
}

.drawer-theme {
  background: none;
  border: none;
  cursor: pointer;
  padding: 4px 0;
  font-family: inherit;

  .theme-ico {
    color: var(--accent-primary);
  }
}

.drawer-resume {
  display: flex;
  align-items: center;
  justify-content: space-between;
  background: var(--accent-primary);
  color: var(--accent-on-primary);
  border-radius: var(--radius-md);
  padding: 12px 18px;
  font-family: var(--font-headline);
  font-size: 0.86rem;
  font-weight: 700;
  text-decoration: none;
  margin-top: 6px;
}

@media (max-width: 860px) {
  .portfolio-toolbar {
    padding: 0 20px;
  }
  .desktop-nav,
  .language-toggle,
  .theme-button,
  .header-cta-btn {
    display: none;
  }
  .mobile-menu-button {
    color: var(--accent-primary);
    display: inline-flex;
    margin-left: auto;
  }
}

@media (max-width: 500px) {
  .portfolio-toolbar {
    padding: 0 16px;
    min-height: 64px;
  }
}
</style>
