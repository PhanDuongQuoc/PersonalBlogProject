<template>
  <section class="simple-map-section">
    <div class="simple-map-container">
      <!-- Minimal Header Bar -->
      <div class="map-top-bar">
        <div class="location-badge">
          <q-icon name="fa-solid fa-location-dot" size="16px" class="loc-icon" />
          <span class="location-text">{{ locationName || 'Thành phố Hồ Chí Minh, Việt Nam' }}</span>
        </div>

        <a
          :href="googleMapsLink"
          target="_blank"
          rel="noopener noreferrer"
          class="open-maps-link"
        >
          <span>{{ text.openInGoogleMaps || 'Mở Google Maps' }}</span>
          <q-icon name="fa-solid fa-arrow-up-right-from-square" size="12px" />
        </a>
      </div>

      <!-- Default Google Map Viewport (Bright / Standard Color) -->
      <div class="map-frame">
        <iframe
          title="Google Map - Ho Chi Minh City"
          class="google-map-iframe"
          :src="mapEmbedUrl"
          width="100%"
          height="100%"
          style="border: 0;"
          allowfullscreen="false"
          loading="lazy"
          referrerpolicy="no-referrer-when-downgrade"
        ></iframe>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';

defineProps<{
  locationName?: string | null;
}>();

const { text } = usePortfolioLocale();

// Google Maps link & embed URL for Ho Chi Minh City (Standard bright view)
const googleMapsLink = 'https://maps.google.com/?q=Ho+Chi+Minh+City,+Vietnam';
const mapEmbedUrl =
  'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d125414.77458156743!2d106.6297!3d10.8231!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x317529292e42c671%3A0x9c4262111d44865!2zVGjDoG5oIHBo4buRIEjhu5MgQ2jDrSBNaW5oLCBWaeG7h3QgTmFt!5e0!3m2!1svi!2s!4v1700000000000!5m2!1svi!2s';
</script>

<style scoped lang="scss">
.simple-map-section {
  width: 100%;
  margin-top: 0;
}

.simple-map-container {
  background: var(--bg-surface-low, #111827);
  border: 1px solid var(--border-hairline, rgba(248, 250, 252, 0.08));
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.25);
  transition: border-color 0.25s ease;

  &:hover {
    border-color: rgba(223, 38, 106, 0.35);
  }
}

.map-top-bar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  padding: 14px 20px;
  background: var(--bg-surface-low, #111827);
  border-bottom: 1px solid var(--border-hairline, rgba(248, 250, 252, 0.08));

  .location-badge {
    display: flex;
    align-items: center;
    gap: 8px;
    color: var(--text-primary, #f8fafc);
    font-family: var(--font-headline, 'Plus Jakarta Sans', sans-serif);
    font-size: 0.92rem;
    font-weight: 600;

    .loc-icon {
      color: var(--accent-primary, #df266a);
      flex-shrink: 0;
    }

    .location-text {
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
  }

  .open-maps-link {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    font-family: var(--font-headline, 'Plus Jakarta Sans', sans-serif);
    font-size: 0.8rem;
    font-weight: 600;
    color: var(--accent-primary, #df266a);
    background: rgba(223, 38, 106, 0.1);
    border: 1px solid rgba(223, 38, 106, 0.25);
    padding: 6px 14px;
    border-radius: 8px;
    text-decoration: none;
    flex-shrink: 0;
    transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);

    &:hover {
      background: var(--accent-primary, #df266a);
      color: #ffffff;
      box-shadow: 0 4px 14px rgba(223, 38, 106, 0.3);
      transform: translateY(-1px);
    }
  }
}

.map-frame {
  width: 100%;
  height: 380px;
  background: #e5e7eb;
  position: relative;

  .google-map-iframe {
    width: 100%;
    height: 100%;
    display: block;
    border: none;
  }
}

@media (max-width: 600px) {
  .map-top-bar {
    flex-direction: column;
    align-items: flex-start;
    gap: 10px;

    .open-maps-link {
      width: 100%;
      justify-content: center;
    }
  }

  .map-frame {
    height: 280px;
  }
}
</style>
