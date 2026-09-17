<template>
  <q-layout view="lHh Lpr lFf" class="portfolio-layout">
    <portfolio-header />

    <q-page-container>
      <router-view />
    </q-page-container>

    <!-- Global Floating AI Chat Widget -->
    <FloatingAiChatWidget />
  </q-layout>
</template>

<script setup lang="ts">
import PortfolioHeader from '@/components/portfolio/PortfolioHeader.vue';
import FloatingAiChatWidget from '@/components/user/chat/FloatingAiChatWidget.vue';
</script>

<style scoped lang="scss">
.portfolio-layout { background: transparent; }
</style>
