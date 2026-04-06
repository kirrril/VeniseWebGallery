function detectShellMode() {
    const params = new URLSearchParams(window.location.search);
    const forcedView = params.get("view");

    if (forcedView === "desktop" || forcedView === "mobile") {
        return forcedView;
    }

    const hasTouchPoints = navigator.maxTouchPoints > 0;
    const coarsePointer = window.matchMedia("(pointer: coarse)").matches;
    const narrowViewport = window.matchMedia("(max-width: 960px)").matches;

    if (hasTouchPoints && (coarsePointer || narrowViewport)) {
        return "mobile";
    }

    return "desktop";
}

function detectMobilePlatform() {
    const userAgent = navigator.userAgent || navigator.vendor || "";

    if (/iPad|iPhone|iPod/i.test(userAgent)) {
        return "ios";
    }

    if (/Android/i.test(userAgent)) {
        return "android";
    }

    return "";
}

function buildTargetUrl(mode, platform) {
    const params = new URLSearchParams(window.location.search);
    params.delete("view");

    if (mode === "mobile" && platform && !params.has("platform")) {
        params.set("platform", platform);
    }

    const query = params.toString();
    const suffix = query ? `?${query}` : "";

    return `${mode}.html${suffix}`;
}

const shellMode = detectShellMode();
const mobilePlatform = shellMode === "mobile" ? detectMobilePlatform() : "";

window.location.replace(buildTargetUrl(shellMode, mobilePlatform));
