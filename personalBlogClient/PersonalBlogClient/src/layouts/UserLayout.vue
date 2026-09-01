<template>
  <q-layout view="lHh Lpr lFf" class="portfolio-layout">
    <portfolio-header />

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script setup lang="ts">
import PortfolioHeader from '@/components/portfolio/PortfolioHeader.vue';
</script>

<style scoped lang="scss">
.portfolio-layout { background: transparent; }
</style>
