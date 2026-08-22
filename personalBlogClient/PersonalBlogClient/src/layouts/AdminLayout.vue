<template>
  <q-layout view="lHh Lpr lFf">
    <q-header elevated class="bg-primary text-white">
      <q-toolbar>
        <q-toolbar-title>Admin Layout</q-toolbar-title>
        <q-btn flat no-caps to="/login" label="Login" />
        <q-btn flat no-caps to="/user" label="User" />
      </q-toolbar>
    </q-header>

    <q-drawer show-if-above bordered :width="220">
      <q-list>
        <q-item-label header>Admin Menu</q-item-label>
        <q-item clickable to="/admin">
          <q-item-section avatar>
            <q-icon name="dashboard" />
          </q-item-section>
          <q-item-section>Dashboard</q-item-section>
        </q-item>
      </q-list>
    </q-drawer>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>
