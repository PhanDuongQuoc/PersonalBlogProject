<template>
  <q-layout view="lHh Lpr lFf">
    <q-header elevated class="bg-dark text-white">
      <q-toolbar>
        <q-toolbar-title>User Layout</q-toolbar-title>
        <q-btn flat no-caps to="/login" label="Login" />
        <q-btn flat no-caps to="/admin" label="Admin" />
      </q-toolbar>
    </q-header>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>
