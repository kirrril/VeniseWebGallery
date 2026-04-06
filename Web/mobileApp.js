const viewButtons = document.querySelectorAll("[data-view-target]");
const overlays = {
    gallery: document.getElementById("gallery-overlay"),
    statement: document.getElementById("statement-overlay"),
    ar: document.getElementById("ar-overlay")
};

const startBtn = document.getElementById("start-btn");
const loadingBar = document.getElementById("loading-bar");
const canvas = document.getElementById("unity-canvas");
const startOverlay = document.getElementById("start-overlay");
const unityContainer = document.getElementById("unity-container");
const siteVersion =
    document.querySelector('meta[name="site-version"]')?.content?.trim() || "dev";
const versionedImageElements = document.querySelectorAll("[data-versioned-src]");
const versionedLinkElements = document.querySelectorAll("[data-versioned-href]");
const storeButton = document.getElementById("mobile-store-btn");
const APP_STORE_URL = "https://testflight.apple.com/join/T2ADyDzF";

let unityInstance = null;

function withVersion(url) {
    const separator = url.includes("?") ? "&" : "?";
    return `${url}${separator}v=${encodeURIComponent(siteVersion)}`;
}

function applySiteVersionToAssets() {
    document.documentElement.style.setProperty("--grid-image", `url("${withVersion("Assets/Grid2.png")}")`);

    versionedImageElements.forEach((element) => {
        const assetPath = element.dataset.versionedSrc;
        if (assetPath) {
            element.src = withVersion(assetPath);
        }
    });

    versionedLinkElements.forEach((element) => {
        const assetPath = element.dataset.versionedHref;
        if (assetPath) {
            element.href = withVersion(assetPath);
        }
    });
}

const config = {
    dataUrl: `BuildMobile/Builds.data?v=${siteVersion}`,
    frameworkUrl: `BuildMobile/Builds.framework.js?v=${siteVersion}`,
    codeUrl: `BuildMobile/Builds.wasm?v=${siteVersion}`,
    streamingAssetsUrl: "StreamingAssets",
    companyName: "kirrril",
    productName: "VeniseARShow",
    productVersion: "1.0"
};

applySiteVersionToAssets();

function getMobilePlatform() {
    const params = new URLSearchParams(window.location.search);
    const platform = params.get("platform");

    if (platform === "ios" || platform === "android") {
        return platform;
    }

    const userAgent = navigator.userAgent || navigator.vendor || "";

    if (/iPad|iPhone|iPod/i.test(userAgent)) {
        return "ios";
    }

    if (/Android/i.test(userAgent)) {
        return "android";
    }

    return "ios";
}

function configureStoreButton() {
    if (!storeButton) {
        return;
    }

    const mobilePlatform = getMobilePlatform();
    storeButton.textContent = mobilePlatform === "android" ? "PlayStore" : "AppStore";
    storeButton.href = APP_STORE_URL;
}

configureStoreButton();

function getFullscreenElement() {
    return document.fullscreenElement || document.webkitFullscreenElement || null;
}

async function toggleFullscreen() {
    try {
        if (getFullscreenElement() === unityContainer) {
            if (document.exitFullscreen) {
                await document.exitFullscreen();
            } else if (document.webkitExitFullscreen) {
                document.webkitExitFullscreen();
            }
        } else if (unityContainer.requestFullscreen) {
            await unityContainer.requestFullscreen();
        } else if (unityContainer.webkitRequestFullscreen) {
            unityContainer.webkitRequestFullscreen();
        }
    } catch (error) {
        console.warn("[Venise] Fullscreen toggle failed.", error);
    }
}

function setActiveView(view) {
    Object.entries(overlays).forEach(([key, overlay]) => {
        const isOpen = key === view;
        overlay.classList.toggle("is-open", isOpen);
        overlay.setAttribute("aria-hidden", String(!isOpen));
    });

    viewButtons.forEach((button) => {
        const isActive = button.dataset.viewTarget === view;
        button.classList.toggle("is-active", isActive);
        button.setAttribute("aria-pressed", String(isActive));
    });
}

viewButtons.forEach((button) => {
    button.addEventListener("click", () => {
        setActiveView(button.dataset.viewTarget);
    });
});

document.querySelectorAll("[data-close-panel]").forEach((button) => {
    button.addEventListener("click", () => {
        setActiveView(null);
    });
});

Object.values(overlays).forEach((overlay) => {
    overlay.addEventListener("click", (event) => {
        if (event.target === overlay) {
            setActiveView(null);
        }
    });
});

startBtn.addEventListener("click", () => {
    startBtn.style.display = "none";
    loadingBar.style.display = "flex";

    createUnityInstance(canvas, config, (progress) => {
        loadingBar.textContent = "Chargement : " + Math.round(progress * 100) + "%";
    }).then((instance) => {
        unityInstance = instance;
        startOverlay.style.display = "none";
    }).catch(() => {
        unityInstance = null;
        loadingBar.textContent = "Erreur de chargement";
        startBtn.style.display = "inline-flex";
        loadingBar.style.display = "none";
    });
});

document.addEventListener("keydown", (event) => {
    if (!unityInstance) {
        return;
    }

    if (event.key.toLowerCase() !== "f") {
        return;
    }

    const targetTag = event.target && event.target.tagName ? event.target.tagName.toLowerCase() : "";
    if (targetTag === "input" || targetTag === "textarea") {
        return;
    }

    event.preventDefault();
    toggleFullscreen();
});
