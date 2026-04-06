# AGENTS.md

## But du depot

Projet Unity cible `WebGL` pour une galerie virtuelle 3D.

Le depot couvre aujourd'hui deux couches complementaires:

- la scene Unity WebGL
- le shell web public de deploiement dans `Web/`

Fonctionnellement, on y trouve:

- une scene 3D navigable
- un joueur en vue libre
- des objets interactifs avec animations et particules
- un site public avec routage `desktop` / `mobile`
- deux builds WebGL deployes separement dans `Web/BuildDesktop/` et `Web/BuildMobile/`

Contexte produit plus large:

- ce projet doit coexister avec un second projet Unity mobile Android/iOS
- le projet mobile doit gerer deplacement mobile, gyro, download d'image targets, impression ou affichage de cibles, puis lancement d'une experience AR
- l'integration des deux projets dans une architecture agentique commune est prevue plus tard

## Regles de collaboration

- On se tutoie.
- Proposer des solutions d'abord.
- Ne jamais appliquer de changement `code`, `scene` ou `ProjectSettings` sans demande explicite de l'utilisateur.
- Si une modification est demandee, rester incremental et limiter le patch au strict necessaire.

## Regles de modification

- Changements minimaux, lisibles, testables.
- Eviter les changements de design systemique si un correctif local suffit.
- Si un comportement depend de l'Inspector, expliciter l'hypothese.
- Ne pas modifier les assets, scenes ou fichiers web non concernes par la demande.
- Quand une documentation semble contredire l'etat du repo, verifier d'abord les fichiers reels avant d'agir.

## Gardes-fous specifiques a ce depot

- Ne pas modifier `Assets/Scenes/MainScene.unity` sans demande explicite: c'est l'unique scene active du build.
- Ne pas modifier `ProjectSettings/*` sans demande explicite: la cible WebGL et les reglages de rendu sont deja configures.
- Ne pas modifier `Assets/Prefabs/PlayerPrefab.prefab` ou `Assets/Prefabs/CanvasOffset.prefab` sans besoin direct: beaucoup de comportements reposent sur leurs references Inspector.
- Considere `Web/` comme la source de verite du site deploye.
- Considere `Web/index.html` comme le point d'entree public, avec `Web/bootstrap.js` pour le routage.
- Considere `Web/desktop.html` + `Web/desktopApp.js` comme la source de verite desktop.
- Considere `Web/mobile.html` + `Web/mobileApp.js` comme la source de verite mobile.
- Considere `Web/BuildDesktop/*` et `Web/BuildMobile/*` comme des artefacts Unity deployes, pas comme des sources a editer a la main sauf demande explicite.
- Considere `Builds/index.html` comme un artefact Unity historique, non source de verite.

## Contexte technique precise

### Editeur Unity

- Editor version: `6000.3.7f1`
- Revision: `6000.3.7f1 (696ec25a53d1)`

Source: `ProjectSettings/ProjectVersion.txt`

### Packages explicites du manifest

- `com.unity.2d.sprite` `1.0.0`
- `com.unity.ai.navigation` `2.0.9`
- `com.unity.cinemachine` `3.1.5`
- `com.unity.collab-proxy` `2.11.3`
- `com.unity.ide.rider` `3.0.39`
- `com.unity.ide.visualstudio` `2.0.26`
- `com.unity.inputsystem` `1.18.0`
- `com.unity.multiplayer.center` `1.0.1`
- `com.unity.render-pipelines.universal` `17.3.0`
- `com.unity.test-framework` `1.6.0`
- `com.unity.timeline` `1.8.10`
- `com.unity.ugui` `2.0.0`
- `com.unity.visualscripting` `1.9.9`

Modules Unity explicites dans `manifest.json`:

- `com.unity.modules.accessibility` `1.0.0`
- `com.unity.modules.adaptiveperformance` `1.0.0`
- `com.unity.modules.ai` `1.0.0`
- `com.unity.modules.androidjni` `1.0.0`
- `com.unity.modules.animation` `1.0.0`
- `com.unity.modules.assetbundle` `1.0.0`
- `com.unity.modules.audio` `1.0.0`
- `com.unity.modules.cloth` `1.0.0`
- `com.unity.modules.director` `1.0.0`
- `com.unity.modules.imageconversion` `1.0.0`
- `com.unity.modules.imgui` `1.0.0`
- `com.unity.modules.jsonserialize` `1.0.0`
- `com.unity.modules.particlesystem` `1.0.0`
- `com.unity.modules.physics` `1.0.0`
- `com.unity.modules.physics2d` `1.0.0`
- `com.unity.modules.screencapture` `1.0.0`
- `com.unity.modules.terrain` `1.0.0`
- `com.unity.modules.terrainphysics` `1.0.0`
- `com.unity.modules.tilemap` `1.0.0`
- `com.unity.modules.ui` `1.0.0`
- `com.unity.modules.uielements` `1.0.0`
- `com.unity.modules.umbra` `1.0.0`
- `com.unity.modules.unityanalytics` `1.0.0`
- `com.unity.modules.unitywebrequest` `1.0.0`
- `com.unity.modules.unitywebrequestassetbundle` `1.0.0`
- `com.unity.modules.unitywebrequestaudio` `1.0.0`
- `com.unity.modules.unitywebrequesttexture` `1.0.0`
- `com.unity.modules.unitywebrequestwww` `1.0.0`
- `com.unity.modules.vectorgraphics` `1.0.0`
- `com.unity.modules.vehicles` `1.0.0`
- `com.unity.modules.video` `1.0.0`
- `com.unity.modules.vr` `1.0.0`
- `com.unity.modules.wind` `1.0.0`
- `com.unity.modules.xr` `1.0.0`

Packages resolus notables d'apres `Packages/packages-lock.json`:

- `com.unity.burst` `1.8.27`
- `com.unity.collections` `2.6.2`
- `com.unity.mathematics` `1.3.3`
- `com.unity.render-pipelines.core` `17.3.0`
- `com.unity.render-pipelines.universal-config` `17.0.3`
- `com.unity.searcher` `4.9.4`
- `com.unity.settings-manager` `2.1.1`
- `com.unity.shadergraph` `17.3.0`
- `com.unity.splines` `2.8.2`
- `com.unity.nuget.mono-cecil` `1.11.6`

### Scene et build

- Scene active unique: `Assets/Scenes/MainScene.unity`
- Build settings: scene unique active
- Template WebGL actuel: `APPLICATION:Default`
- Export Unity historique present: `Builds/`
- Builds deployes actuels pour le site: `Web/BuildDesktop/` et `Web/BuildMobile/`

### Rendu

- Pipeline: `URP`
- Render pipeline global actif: `Assets/Settings/PC_RPAsset.asset`
- Qualite par defaut y compris `WebGL`: profil `PC`
- Un asset `Assets/Settings/Mobile_RPAsset.asset` existe, mais il n'est pas la pipeline active WebGL actuelle

### Reglages WebGL importants

- Resolution Web par defaut: `1920x1080`
- Color space: `Linear`
- `webGLTemplate: APPLICATION:Default`
- `webGLDataCaching: 1`
- `webGLThreadsSupport: 0`
- `webGLInitialMemorySize: 512`
- `webGLMaximumMemorySize: 2048`
- `webGLCompressionFormat: 2`
- `webGLDecompressionFallback: 0`
- `activeInputHandler: 1` donc `Input System` moderne actif seul

## Architecture fonctionnelle actuelle

### Scene

La scene active contient au minimum les objets suivants:

- `Main Camera`
- `SceneManager`
- `EventSystem`
- `EntryPoint`
- une instance de `PlayerPrefab`

### Scripts gameplay

- `SceneManager.cs`
  - verrouille et cache le curseur
  - fixe `Application.targetFrameRate = 30`
- `PlayerController.cs`
  - deplace un `Rigidbody`
  - lit les actions `Move` et `Look` du `Input System`
  - depend de references Inspector: `rb`, `cameraTarget`, `cameraPlace`, `entryPoint`
  - positionne le joueur sur `entryPoint` au `Start()`
- `SphereController.cs`
  - active une animation de rebond et un canvas a l'entree du trigger
- `RectangleController.cs`
  - active une animation de torsion et un canvas a l'entree du trigger
- `ShowerController.cs`
  - active trois systemes de particules et un canvas a l'entree du trigger
- `PlayDustBurst.cs`
  - declenche deux systemes de particules
- `CanvasRotation.cs`
  - oriente un canvas vers `Camera.main`

### Prefabs et dependances Inspector

- `Assets/Prefabs/PlayerPrefab.prefab`
  - contient `PlayerInput`, `PlayerController`, `Rigidbody`, `CapsuleCollider`
  - `entryPoint` est nul dans le prefab et doit etre injecte au niveau scene ou prefab instance
  - `cameraTarget` et `cameraPlace` sont des enfants du prefab
- `Assets/Prefabs/CanvasOffset.prefab`
  - contient un canvas monde + `CanvasRotation`
  - sert de bulle d'information contextuelle pres des objets

### Input

- Asset d'input: `Assets/InputSystem_Actions.inputactions`
- Action map `Player`: `Move`, `Look`, `Attack`, `Interact`, `Jump`
- Action map `UI` egalement presente
- Control schemes declares: `Keyboard&Mouse`, `Gamepad`, `Touch`, `Joystick`, `XR`
- En pratique, le gameplay code actuel ne traite explicitement que `Move` et `Look`

## Convention de travail pour le WebGL et le site

- `Web/index.html` est le point d'entree public du site.
- `Web/bootstrap.js` detecte le profil client puis redirige vers `desktop.html` ou `mobile.html`.
- `Web/bootstrap.js` detecte aussi `ios` ou `android` pour la branche mobile et transmet cette information via le query string.
- `Web/desktop.html` charge `BuildDesktop/Builds.loader.js` et `Web/desktopApp.js`.
- `Web/mobile.html` charge `BuildMobile/Builds.loader.js` et `Web/mobileApp.js`.
- `desktop.html` et `mobile.html` portent chacune leur propre `meta[name="site-version"]`.
- `desktopApp.js` et `mobileApp.js` versionnent aussi les assets web references via `data-versioned-src` et `data-versioned-href`.
- `mobile.html` fonctionne actuellement comme un hub mobile:
  - page d'accueil avec logo + 3 boutons
  - `Web gallery`, `Statement` et `AR Experience` ouvrent des overlays plein ecran
  - l'overlay `AR Experience` n'affiche plus de QR codes et utilise un bouton store unique
- `Web/.htaccess` sert les shells et scripts en `no-cache, must-revalidate`.
- `Web/.htaccess` sert `BuildDesktop/*.(data|wasm|js)` et `BuildMobile/*.(data|wasm|js)` avec cache long.

## Convention de travail pour le fullscreen et le cursor lock

- le fullscreen desktop appartient au shell web, pas au build Unity
- la logique active de fullscreen est dans `Web/desktopApp.js`
- la touche `F` y alterne le fullscreen du conteneur Unity
- le cursor lock reste une intention cote Unity via `SceneManager.cs`
- en WebGL, le navigateur garde l'autorite finale sur le pointer lock
- sur mobile, les appels Unity de cursor lock sont inutiles mais non bloquants a ce stade
- sur mobile, l'ouverture du lien store peut rester dans le meme onglet, ouvrir un nouvel onglet ou basculer vers une vue systeme selon le navigateur et l'OS

## Hypotheses a expliciter si un agent modifie le projet

- si un comportement depend d'une reference de scene, nommer l'objet exact et le champ Inspector exact
- si un changement touche le WebGL, preciser s'il vise:
  - le build Unity genere
  - le shell web public
  - la logique de routage `desktop` / `mobile`
  - la logique de deploiement sur le VPS OVH
- si un changement vise la navigation, preciser s'il concerne clavier et souris desktop seulement ou aussi mobile, gamepad ou touch

## Etat local a garder en tete

- le worktree n'est pas propre
- `AGENTS.md`, `PlayerController.cs`, `MainScene.unity`, `ProjectSettings/ProjectSettings.asset`, `NOTES.md`, `Web/.htaccess` et `Web/index.html` sont deja modifies localement
- `Assets/Scripts/WebClientBootstrap.cs` et son `.meta` sont supprimes localement
- `Web/bootstrap.js`, `Web/desktop.html`, `Web/desktopApp.js`, `Web/mobile.html`, `Web/mobileApp.js`, `Web/BuildDesktop/`, `Web/BuildMobile/` et `Web/VeniceTargets.pdf` sont actuellement non suivis
- eviter toute edition large sans verifier d'abord l'origine des changements deja presents

## Points d'attention actuels

- certaines documentations historiques peuvent encore mentionner `SampleScene` ou `Assets/Scripts/index.html`: ce n'est plus l'etat reel du repo
- `desktopApp.js` et `mobileApp.js` restent tres proches et dupliques
- `desktop.html` et `mobile.html` divergent maintenant nettement dans leur UX, mais partagent encore une base de styles et de structure
- `productName` vaut encore `VeniseARShow` dans les apps web, alors que le projet Unity est `Venise_WebGallery`
- la logique gameplay Unity reste desktop-first, meme si le shell web sait maintenant router vers une branche mobile
- la prochaine etape structurante attendue est la creation d'une vraie scene mobile Unity, idealement accompagnee d'une scene desktop dediee et d'un workflow de build distinct
