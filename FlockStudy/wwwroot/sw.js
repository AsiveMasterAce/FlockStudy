const CACHE_NAME = 'flock-app-v1';
const urlsToCache = [
    '/',
    '/css/site.css',
    '/js/site.js',
    '/icons/icon-192x192.png',
    '/icons/icon-512x512.png'
];

self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => cache.addAll(urlsToCache))
    );
});

self.addEventListener('fetch', event => {
    event.respondWith(
        caches.match(event.request)
            .then(response => {
                if (response) {
                    return response;
                }
                return fetch(event.request);
            })
    );
});

// Push notification handling
self.addEventListener('push', event => {
    const options = {
        body: event.data ? event.data.text() : 'New prayer request needs your attention!',
        icon: '/icons/icon-192x192.png',
        badge: '/icons/icon-192x192.png'
    };

    event.waitUntil(
        self.registration.showNotification('Prayer App', options)
    );
});