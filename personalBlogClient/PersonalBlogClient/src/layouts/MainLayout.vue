<template>
  <q-layout view="lHh Lpr lFf">
    <q-header elevated>
      <q-toolbar>
        <q-btn
          flat
          dense
          round
          icon="fa-solid fa-bars"
          aria-label="Menu"
          @click="toggleLeftDrawer"
        />

        <q-toolbar-title> Quasar App </q-toolbar-title>

        <div>Quasar v{{ $q.version }}</div>
      </q-toolbar>
    </q-header>

    <q-drawer v-model="leftDrawerOpen" show-if-above bordered>
      <q-list>
        <q-item-label header> Essential Links </q-item-label>

        <EssentialLink
          v-for="link in linksList"
          :key="link.label"
          v-bind="link"
        />
      </q-list>
    </q-drawer>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script setup lang="ts">
import { ref } from "vue";
import EssentialLink, {
  type EssentialLinkProps
} from "@/components/EssentialLink.vue";

const linksList: EssentialLinkProps[] = [
  {
    label: "Docs",
    caption: "quasar.dev",
    icon: "fa-solid fa-graduation-cap",
    link: "https://quasar.dev"
  },
  {
    label: "GitHub",
    caption: "github.com/quasarframework",
    icon: "fa-brands fa-github",
    link: "https://github.com/quasarframework"
  },
  {
    label: "Discord Chat Channel",
    caption: "chat.quasar.dev",
    icon: "fa-brands fa-discord",
    link: "https://chat.quasar.dev"
  },
  {
    label: "Forum",
    caption: "forum.quasar.dev",
    icon: "fa-solid fa-comments",
    link: "https://forum.quasar.dev"
  },
  {
    label: "Twitter",
    caption: "@quasarframework",
    icon: "fa-brands fa-x-twitter",
    link: "https://twitter.quasar.dev"
  },
  {
    label: "Facebook",
    caption: "@QuasarFramework",
    icon: "fa-brands fa-facebook",
    link: "https://facebook.quasar.dev"
  },
  {
    label: "Quasar Awesome",
    caption: "Community Quasar projects",
    icon: "fa-solid fa-heart",
    link: "https://awesome.quasar.dev"
  }
];

const leftDrawerOpen = ref(false);

function toggleLeftDrawer() {
  leftDrawerOpen.value = !leftDrawerOpen.value;
}
</script>
