
// PWA Install Prompt Logic
    let deferredPrompt;
    let isInstalled = false;

    // Check if already installed
    if (window.matchMedia('(display-mode: standalone)').matches || window.navigator.standalone) {
        isInstalled = true;
}

// Listen for beforeinstallprompt event
window.addEventListener('beforeinstallprompt', (e) => {
        e.preventDefault();
    deferredPrompt = e;

    if (!isInstalled && !localStorage.getItem('installDismissed')) {
        showInstallPrompt();
    }
});

    // Detect iOS
    function isIOS() {
    return /iPad|iPhone|iPod/.test(navigator.userAgent) && !window.MSStream;
}

    // Detect Android
    function isAndroid() {
    return /Android/.test(navigator.userAgent);
}

    // Show appropriate install prompt
    function showInstallPrompt() {
        // Show banner after 3 seconds
        setTimeout(() => {
            if (!isInstalled) {
                document.getElementById('installBanner').style.display = 'block';

                // Show floating button after 10 seconds if banner is dismissed
                setTimeout(() => {
                    if (!isInstalled && localStorage.getItem('bannerDismissed')) {
                        document.getElementById('floatingInstall').style.display = 'block';
                    }
                }, 10000);
            }
        }, 3000);
}

    // Show install modal
    function showInstallModal() {
    const modal = document.getElementById('installModal');
    const iosInstructions = document.getElementById('iosInstructions');
    const androidInstructions = document.getElementById('androidInstructions');
    const autoInstallText = document.getElementById('autoInstallText');
    const manualInstallText = document.getElementById('manualInstallText');

    modal.style.display = 'flex';

    // Show appropriate instructions
    if (isIOS()) {
        iosInstructions.style.display = 'block';
    autoInstallText.style.display = 'none';
    manualInstallText.style.display = 'inline';
    } else if (isAndroid()) {
        androidInstructions.style.display = 'block';
    if (!deferredPrompt) {
        autoInstallText.style.display = 'none';
    manualInstallText.style.display = 'inline';
        }
    }
}

    // Install the app
    async function installApp() {
    if (deferredPrompt) {
        deferredPrompt.prompt();
    const {outcome} = await deferredPrompt.userChoice;

    if (outcome === 'accepted') {
        deferredPrompt = null;
    hideInstallPrompts();
        }
    } else {
        // For iOS or manual installation
        hideInstallPrompts();
    }
}

    // Hide all install prompts
    function hideInstallPrompts() {
    const banner = document.getElementById('installBanner');
    const modal = document.getElementById('installModal');
    const floating = document.getElementById('floatingInstall');

    banner.style.display = 'none';
    modal.style.display = 'none';
    floating.style.display = 'none';

    localStorage.setItem('installDismissed', 'true');
}

    // Event listeners
    document.getElementById('installButton').addEventListener('click', showInstallModal);
    document.getElementById('installAppButton').addEventListener('click', installApp);
    document.getElementById('floatingInstall').addEventListener('click', showInstallModal);

document.getElementById('dismissBanner').addEventListener('click', () => {
    const banner = document.getElementById('installBanner');
    banner.classList.add('fade-out');
    setTimeout(() => {
        banner.style.display = 'none';
    localStorage.setItem('bannerDismissed', 'true');

        // Show floating button after dismissing banner
        setTimeout(() => {
            if (!isInstalled) {
        document.getElementById('floatingInstall').style.display = 'block';
            }
        }, 5000);
    }, 300);
});

document.getElementById('closeModal').addEventListener('click', () => {
        document.getElementById('installModal').style.display = 'none';
});

// Close modal when clicking outside
document.getElementById('installModal').addEventListener('click', (e) => {
    if (e.target === e.currentTarget) {
        document.getElementById('installModal').style.display = 'none';
    }
});

// Listen for app installed event
window.addEventListener('appinstalled', () => {
        isInstalled = true;
    hideInstallPrompts();
    console.log('PWA was installed');
});

    // Auto-show prompt for demonstration
 showInstallPrompt();
