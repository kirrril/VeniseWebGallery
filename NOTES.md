# NOTES.md

## Analyse du projet Web gallery

## 1. Resume executif

Le projet est aujourd'hui coherent autour de deux couches bien distinctes:

- une base Unity WebGL encore desktop-first
- un shell web de deploiement dans `Web/` qui separe deja les experiences `desktop` et `mobile`

Le coeur Unity reste simple et exploitable:

- scene unique `MainScene`
- joueur libre
- objets interactifs locaux
- rendu URP
- export WebGL fonctionnel

La couche web, elle, a nettement evolue:

- `Web/index.html` + `bootstrap.js` servent de point d'entree unique
- `desktop.html` et `mobile.html` sont maintenant de vraies branches de shell distinctes
- `BuildDesktop/` et `BuildMobile/` sont deja separes cote deploiement
- le pont JS -> Unity a ete retire

Conclusion courte:

- l'architecture web est maintenant suffisamment claire pour supporter deux variantes produit
- la prochaine grosse etape logique est bien cote Unity:
  - creer `DesktopScene`
  - creer `MobileScene`
  - produire deux builds Unity distincts en coherence avec les deux shells web

## 2. Cartographie du depot

- `Assets/Scenes/MainScene.unity`
  - scene active actuelle du build
- `Assets/Prefabs/PlayerPrefab.prefab`
  - prefab joueur avec `PlayerInput`, `PlayerController`, `Rigidbody`
- `Assets/Prefabs/CanvasOffset.prefab`
  - prefab de canvas monde oriente vers la camera
- `Assets/Scripts/`
  - scripts gameplay uniquement
  - le script `WebClientBootstrap.cs` n'existe plus
- `Web/`
  - source de verite du site deploye
  - contient `index.html`, `bootstrap.js`, `desktop.html`, `desktopApp.js`, `mobile.html`, `mobileApp.js`, `Assets/`, `BuildDesktop/`, `BuildMobile/`, `VeniceTargets.pdf`
- `Assets/Settings/`
  - assets URP `PC` et `Mobile`
- `Builds/`
  - export Unity historique
  - utile comme artefact technique, plus comme source de verite du site
- `ProjectSettings/`
  - configuration WebGL, rendu, input, qualite

## 3. Versions et stack

- Unity Editor: `6000.3.7f1 (696ec25a53d1)`
- Render pipeline: `URP 17.3.0`
- Input: `Input System 1.18.0`
- Camera system: `Cinemachine 3.1.5`
- Navigation package: `AI Navigation 2.0.9`
- UI: `UGUI 2.0.0`
- Timeline: `1.8.10`
- Visual Scripting: `1.9.9`

Packages resolus notables:

- `Burst 1.8.27`
- `Collections 2.6.2`
- `Mathematics 1.3.3`
- `Splines 2.8.2`

## 4. Architecture fonctionnelle actuelle

### Scene Unity

La scene active contient au minimum:

- `Main Camera`
- `SceneManager`
- `EventSystem`
- `EntryPoint`
- une instance de `PlayerPrefab`

Le GameObject `WebClientBootstrap` a ete retire de la scene.

### Joueur

Le joueur repose sur:

- `PlayerInput`
- `PlayerController`
- `Rigidbody`
- `CapsuleCollider`
- `CameraTarget`
- `CameraPlace`

Point important:

- `entryPoint` est nul dans `PlayerPrefab.prefab`
- il doit toujours etre injecte au niveau scene ou instance prefab

### Interactions

Les interactions restent locales et lisibles:

- entree dans un trigger
- activation d'animation, canvas ou particules
- remise a l'etat initial en sortie

Cette structure reste adaptee a une galerie, mais repose fortement sur l'Inspector.

## 5. Lecture des scripts

### `SceneManager.cs`

Role actuel:

- verrouille le curseur
- masque le curseur
- fixe `Application.targetFrameRate = 30`

Lecture produit:

- pertinent pour la build desktop actuelle
- encore acceptable tant que la scene Unity reste essentiellement desktop-first
- a reevaluer quand une vraie `MobileScene` existera

### `PlayerController.cs`

Etat actuel:

- mouvement par `Rigidbody.linearVelocity`
- rotation du corps par `rb.MoveRotation`
- pitch borne entre `minPitch` et `maxPitch`
- `Move` et `Look` seulement sont traites

Point d'attention:

- le joueur depend toujours de plusieurs references Inspector critiques:
  - `rb`
  - `cameraTarget`
  - `cameraPlace`
  - `entryPoint`

### Scripts d'interaction

- `SphereController.cs`
- `RectangleController.cs`
- `ShowerController.cs`
- `PlayDustBurst.cs`
- `CanvasRotation.cs`

Ils restent simples, locaux et faciles a auditer.

## 6. Input et plateforme

Le projet Unity est toujours configure pour le nouveau `Input System` uniquement.

Points observes:

- action map `Player` avec `Move`, `Look`, `Attack`, `Interact`, `Jump`
- action map `UI`
- control schemes declares pour `Keyboard&Mouse`, `Gamepad`, `Touch`, `Joystick`, `XR`

Mais en pratique:

- seul `Move` et `Look` sont branches au gameplay
- il n'y a pas encore de gameplay mobile Unity dans ce depot

Conclusion:

- la base Unity reste desktop-first
- la couche web mobile existe deja
- la vraie variante mobile Unity reste a construire

## 7. Rendu et configuration WebGL

Constats principaux:

- WebGL utilise le profil de qualite `PC`
- la pipeline active est `PC_RPAsset`
- `Mobile_RPAsset` existe mais n'est pas actif pour la build WebGL actuelle
- color space `Linear`
- resolution web par defaut `1920x1080`
- `webGLDataCaching` actif
- `webGLThreadsSupport` desactive
- memoire initiale `512`
- memoire max `2048`
- compression Unity desactivee, compression `gzip` geree cote serveur

Lecture produit:

- cohérent pour la branche desktop actuelle
- probablement trop uniforme pour une vraie branche mobile Unity a terme

## 8. Analyse de la couche web

### Point d'entree et routage

Le point d'entree public est:

- `Web/index.html`
- `Web/bootstrap.js`

`bootstrap.js`:

- detecte `desktop` ou `mobile`
- detecte aussi la plateforme mobile `ios` ou `android`
- transmet cette information a `mobile.html` via le query string

### Branche desktop

La version desktop actuelle repose sur:

- `Web/desktop.html`
- `Web/desktopApp.js`
- `Web/BuildDesktop/`

Caracteristiques actuelles:

- shell classique avec galerie visible dans la page
- panneaux `Statement` et `AR Experience`
- fullscreen gere par la touche `F`
- QR codes encore presents dans le panneau AR desktop

### Branche mobile

La version mobile actuelle repose sur:

- `Web/mobile.html`
- `Web/mobileApp.js`
- `Web/BuildMobile/`

Caracteristiques actuelles:

- page d'accueil mobile avec logo et 3 boutons verticaux
- `Web gallery`, `Statement` et `AR Experience` ouvrent chacun un overlay plein ecran
- la galerie Unity est elle aussi chargee dans un overlay fullscreen
- le bouton `Close` est reduit a 70% sur les overlays
- le panneau `AR Experience` n'affiche plus de QR codes
- un seul bouton store est affiche
- le label du bouton devient:
  - `AppStore` sur iOS
  - `PlayStore` sur Android
- dans les deux cas, le lien pointe vers TestFlight:
  - `https://testflight.apple.com/join/T2ADyDzF`

### Point important d'architecture

Le pont JS -> Unity n'est plus necessaire dans l'architecture actuelle.

Il a ete retire:

- plus de `WebClientBootstrap.cs`
- plus de `SendMessage` depuis les shells web
- le choix `desktop` / `mobile` est maintenant resolu avant le lancement du build Unity

Cette decision est coherente avec la strategie cible:

- un shell desktop
- un shell mobile
- a terme deux scenes Unity distinctes
- a terme deux builds Unity vraiment differents

## 9. Deploiement et serveur

Etat de deploiement retenu:

- `Web/` est le miroir local du serveur
- le deploiement copie tout `Web/` vers `/var/www/venise.kirrril.com/`

Arborescence serveur cible:

```text
/var/www/venise.kirrril.com/
|- .htaccess
|- index.html
|- bootstrap.js
|- desktop.html
|- mobile.html
|- desktopApp.js
|- mobileApp.js
|- Assets/
|- VeniceTargets.pdf
|- BuildDesktop/
|  |- Builds.loader.js
|  |- Builds.framework.js
|  |- Builds.data
|  `- Builds.wasm
`- BuildMobile/
   |- Builds.loader.js
   |- Builds.framework.js
   |- Builds.data
   `- Builds.wasm
```

Headers et cache retenus:

- `index.html`, `desktop.html`, `mobile.html`, `bootstrap.js`, `desktopApp.js`, `mobileApp.js` en `no-cache, must-revalidate`
- `BuildDesktop/*.(data|wasm|js)` et `BuildMobile/*.(data|wasm|js)` en cache long
- MIME `wasm`, `js`, `data` declares dans `.htaccess`
- compression `gzip` geree par Apache

## 10. Risques et dettes techniques prioritaires

### 1. Une seule scene Unity existe encore

Le web est deja separe, mais Unity ne l'est pas encore reellement.

Risque:

- le shell mobile peut diverger plus vite que la scene Unity
- on continue a faire tourner une base desktop dans une experience mobile

### 2. Reglages Unity encore globaux

Meme avec deux shells et deux builds de deploiement, les `PlayerSettings` restent globaux au projet.

Risque:

- si desktop et mobile doivent diverger sur memoire, rendu, compression ou template, il faudra un vrai workflow de build distinct

### 3. Forte dependance Inspector

Le comportement Unity depend encore beaucoup de:

- references scene
- triggers
- animators
- canvases
- particle systems

Risque:

- une petite modification de scene ou prefab peut casser un flux sans erreur de compilation

### 4. Nomenclature partiellement incoherente

Constat:

- le projet Unity s'appelle `Venise_WebGallery`
- les shells web utilisent encore `Venise AR Show`
- `productName` des configs JS vaut encore `VeniseARShow`

Risque:

- confusion produit et technique pendant les futures sessions

### 5. Pas de tests automatises

Risque:

- regressions silencieuses sur input, scene, overlays ou deploiement

## 11. Forces du projet

- separation web `desktop` / `mobile` maintenant claire
- point d'entree unique simple a maintenir
- build desktop deja exploitable
- shell mobile deja tres avance pour le produit
- scene Unity courte et lisible
- interactions locales faciles a auditer
- bon socle pour evoluer vers deux scenes Unity dediees

## 12. Etat local observe

Le worktree local n'est pas propre.

Etat observe au moment de cette note:

- `AGENTS.md` est modifie
- `Assets/Scenes/MainScene.unity` est modifie
- `Assets/Scripts/PlayerController.cs` est modifie
- `Assets/Scripts/WebClientBootstrap.cs` et son `.meta` sont supprimes
- `NOTES.md` est modifie
- `ProjectSettings/ProjectSettings.asset` est modifie
- `Web/.htaccess` est modifie
- `Web/app.js` est supprime
- `Web/index.html` est modifie
- `Web/bootstrap.js`, `Web/desktop.html`, `Web/desktopApp.js`, `Web/mobile.html`, `Web/mobileApp.js`, `Web/BuildDesktop/`, `Web/BuildMobile/`, `Web/VeniceTargets.pdf` sont presents localement et non suivis

Conseil:

- considerer le depot comme etant en transition structurelle active
- eviter toute edition large de scene, prefab ou `ProjectSettings` sans valider d'abord les changements deja presents

## 13. Prochaines etapes utiles

- creer une vraie `MobileScene` Unity
- creer une vraie `DesktopScene` Unity si la separation devient franche
- mettre en place un workflow de build distinct pour les deux variantes
- decider si la galerie mobile doit rester un overlay ou devenir une page dediee
- resynchroniser la nomenclature produit entre Unity, shell web et deploiement
- auditer ensuite les performances de la variante mobile Unity

## 14. Etat reel retenu a ce jour

Pour eviter toute ambiguite dans les prochaines sessions, les conventions a retenir sont:

- scene active actuelle: `Assets/Scenes/MainScene.unity`
- shell web source de verite: `Web/`
- point d'entree public: `Web/index.html` + `Web/bootstrap.js`
- shell desktop public: `Web/desktop.html` + `Web/desktopApp.js`
- shell mobile public: `Web/mobile.html` + `Web/mobileApp.js`
- builds deployes: `Web/BuildDesktop/` et `Web/BuildMobile/`
- pont web -> Unity: supprime
- fullscreen desktop: gere par le shell web
- galerie mobile: ouverte via overlay fullscreen
- AR mobile: bouton store unique avec label dependant de la plateforme
- prochaine vraie etape structurelle: separer aussi Unity en scenes et builds dedies
