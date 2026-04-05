# NOTES.md

## Analyse du projet Web gallery

## 1. Resume executif

Le projet est une base Unity WebGL simple, lisible et deja exploitable pour une galerie virtuelle 3D. Il repose actuellement sur une scene active `Assets/Scenes/MainScene.unity`, un seul prefab joueur, quelques objets interactifs declenches par triggers, un export WebGL deja present dans `Builds/`, et un shell web de deploiement maintenu dans `Web/`.

Le coeur de la scene est sain pour un prototype ou une premiere experience publique:

- navigation clavier/souris
- rendu URP
- animations et particules
- UI contextuelle en world space
- build WebGL exporte

En revanche, l'ensemble reste encore au stade "prototype vertical slice" sur plusieurs points:

- le shell web desktop est en cours de stabilisation
- la documentation du depot n'est pas encore totalement resynchronisee avec l'etat reel
- la logique de navigation est rudimentaire
- plusieurs comportements reposent sur l'Inspector
- il n'y a pas de tests automatises

## 2. Cartographie du depot

- `Assets/Scenes/MainScene.unity`
  - scene active actuelle du build d'apres `ProjectSettings/EditorBuildSettings.asset`
- `Assets/Prefabs/PlayerPrefab.prefab`
  - prefab du joueur avec `PlayerInput`, `PlayerController`, `Rigidbody`
- `Assets/Prefabs/CanvasOffset.prefab`
  - prefab de canvas monde oriente vers la camera
- `Assets/Scripts/`
  - scripts gameplay + `WebClientBootstrap.cs` pour le pont web -> Unity
- `Web/`
  - shell web de deploiement a maintenir en source de verite pour le site public
- `Assets/Settings/`
  - assets URP `PC` et `Mobile`
- `Builds/`
  - export WebGL deja genere avec `Build/`, `TemplateData/` et `index.html`
- `ProjectSettings/`
  - configuration WebGL, URP, Input System, qualite

## 3. Versions et stack

- Unity Editor: `6000.3.7f1 (696ec25a53d1)`
- Render pipeline: `URP 17.3.0`
- Input: `Input System 1.18.0`
- Camera system: `Cinemachine 3.1.5`
- Navigation package installe: `AI Navigation 2.0.9`
- UI: `UGUI 2.0.0`
- Timeline: `1.8.10`
- Visual Scripting: `1.9.9`

Packages resolves notables:

- `Burst 1.8.27`
- `Collections 2.6.2`
- `Mathematics 1.3.3`
- `Splines 2.8.2`

## 4. Architecture fonctionnelle

### Scene

La scene contient:

- une camera principale avec `CinemachineBrain`
- un objet `SceneManager`
- un objet `WebClientBootstrap`
- un `EventSystem`
- un `EntryPoint`
- un prefab joueur
- trois zones d'interaction principales:
  - sculpture sphere
  - sculpture rectangulaire
  - douche/installation a particules

### Joueur

Le joueur repose sur:

- `PlayerInput`
- `PlayerController`
- `Rigidbody`
- `CapsuleCollider`
- un `CameraTarget`
- un `CameraPlace`
- un `PersonalCanvas` desactive par defaut

Point important:

- le champ `entryPoint` est nul dans `PlayerPrefab.prefab`
- il doit donc etre branche via une instance prefab en scene ou par une autre etape d'assemblage Inspector

### Interactions

Les interactions sont tres directes:

- entree dans un trigger
- animation/particules activees
- canvas d'information active
- sortie du trigger
- retour a l'etat initial

Ce schema est simple et robuste pour une galerie, mais il depend fortement de la configuration Inspector.

## 5. Lecture des scripts

### `SceneManager.cs`

Role:

- verrouille le curseur
- masque le curseur
- force `Application.targetFrameRate = 30`

Impact:

- bon pour une experience controllable
- potentiellement limitant pour WebGL desktop si l'objectif est une navigation plus fluide
- ce cap est neanmoins assume a ce stade pour garder un budget de performance plus stable
- justification produit actuelle:
  - la build WebGL utilise un asset `URP PC` et non un asset mobile
  - ce choix est lie a la qualite visuelle recherchee, notamment pour les ombres et l'ambient occlusion
  - ce meme build doit aussi servir de base a la future version mobile du site
  - le cap `30 FPS` sert donc de garde-fou de performance transversal plutot que d'optimisation desktop pure

### `PlayerController.cs`

Points positifs:

- implementation courte
- flux clair
- dependances explicites

Points d'attention:

- le mouvement utilise directement `Rigidbody.linearVelocity`
- la rotation utilise `Rigidbody.angularVelocity`
- `mouseDelta` est normalise avant usage
- `pitch` n'est pas clamp globalement apres accumulation
- le code contient des `using` inutilises et un `OnTriggerEnter` vide

Consequence probable:

- la sensation de souris risque d'etre peu precise, avec une vitesse de rotation presque binaire
- la camera verticale peut deriver au lieu d'etre bornee proprement

### `SphereController.cs`, `RectangleController.cs`, `ShowerController.cs`

Ces scripts sont adaptes a un projet de galerie:

- logique locale
- faible couplage
- comportement facile a comprendre

Limite:

- pas de couche d'abstraction ou de systeme generique d'interaction

### `CanvasRotation.cs`

Bonne solution pour une UI monde minimale:

- le canvas suit l'orientation de `Camera.main`

Limite:

- dependance implicite a la camera taggee `MainCamera`

## 6. Input et plateforme

Le projet est configure pour le nouveau `Input System` uniquement.

Points observes:

- action map `Player` avec `Move`, `Look`, `Attack`, `Interact`, `Jump`
- action map `UI`
- control schemes declares pour `Keyboard&Mouse`, `Gamepad`, `Touch`, `Joystick`, `XR`

Mais en pratique:

- seul `Move` et `Look` sont branches au gameplay actuel
- il n'y a pas de logique de controle mobile/touch dans ce depot
- cela est coherent avec le fait que le vrai projet mobile est separe

Conclusion:

- le projet WebGL est clairement pense desktop-first
- l'ecosysteme mobile/AR est conceptuellement prevu, mais pas implemente ici

## 7. Rendu et configuration WebGL

Constats principaux:

- WebGL utilise le profil de qualite `PC`
- la pipeline active est `PC_RPAsset`
- `Mobile_RPAsset` existe mais n'est pas active pour WebGL
- color space `Linear`
- resolution web par defaut `1920x1080`
- `Compression Format` est regle sur `Disabled` dans `Player > WebGL > Publishing Settings`
- `webGLDataCaching` active
- `webGLThreadsSupport` desactive
- memoire initiale `512`
- memoire max `2048`
- batching `WebGL` desactive dans `PlayerSettings`

Lecture produit:

- le projet cherche davantage une fidelite visuelle simple qu'une optimisation WebGL agressive
- c'est acceptable pour un prototype desktop, moins pour une cible large ou des machines modestes
- le build Unity n'est pas precompresse par Unity et laisse au serveur Apache la compression `gzip` a la livraison

## 8. Analyse de la couche web

### Point cle

Il y a deux fichiers `index.html` a ne pas confondre:

- `Builds/index.html`
  - page generee par Unity pour l'export WebGL
- `Web/index.html`
  - vrai fichier HTML source a maintenir pour le site de deploiement

### Ce que dit la configuration Unity

Le projet est regle sur:

- `webGLTemplate: APPLICATION:Default`

Donc:

- `Web/index.html` n'est pas utilise automatiquement comme template WebGL par la configuration Unity actuelle
- toute regeneration du build continue de produire `Builds/index.html` selon le template par defaut, sauf changement explicite de strategie

Mais, convention de projet confirmee:

- le vrai `index.html` du site a faire evoluer est `Web/index.html`
- `Builds/index.html` reste un fichier technique genere automatiquement par Unity

### Analyse de `Builds/index.html`

Forces:

- page fonctionnelle standard Unity
- barre de chargement
- bouton fullscreen
- gestion de banniere warning/error

Faiblesses:

- aspect "build technique", pas "site produit"
- dimensions desktop fixees a `1920x1080`
- peu responsive pour un site public classique
- aucun contenu editorial sur la galerie, le projet, l'AR ou la navigation
- risque d'ecrasement a chaque nouveau build

Conclusion:

- bon point d'entree technique
- mauvaise base pour un site public soigne si on l'edite directement

### Analyse de `Web/index.html` et `Web/app.js`

Forces:

- intention claire de faire une page d'accueil plus accueillante
- overlay de demarrage
- CTA `Enter the gallery`
- chargement au clic, utile pour l'audio/pointer lock et l'experience immersive
- design sobre et coherent avec une galerie numerique
- versioning de deploiement simple via `meta[name="site-version"]`
- fullscreen desktop gere par le shell web
- fond et assets web versionnes depuis `Web/Assets/*`
- presence d'un pont JS -> Unity pour transmettre le mode client (`desktop` / `mobile`)
- conventions desktop deja clarifiees pour les hints d'usage, le versioning et le fullscreen

Faiblesses:

- pas de gestion d'erreur si `createUnityInstance` echoue
- pas de gestion explicite du warning banner Unity
- pas de contenu de presentation autour du canvas
- pas de SEO, pas de meta description, pas de partage social
- `productName` y vaut `VeniseARShow` alors que le projet Unity est `Venise_WebGallery`, signe de derive nomenclature
- le mode mobile n'est pour l'instant detecte et transmis qu'au niveau bootstrap, sans UI ou gameplay mobile encore branches dans la scene

Conclusion:

- `Web/index.html` et `Web/app.js` sont maintenant la source de verite de la version web/desktop du site
- ce shell web est correctement separe du template WebGL Unity par defaut
- la prochaine etape n'est plus de clarifier le point d'entree web, mais de stabiliser la desktop puis d'ouvrir un vrai mode mobile

## 9. Recommandation pour le futur travail sur le site

Ordre recommande:

1. Garder `Web/` comme source de verite du shell web.
2. Stabiliser l'experience desktop validee sans casser le build Unity.
3. N'ouvrir le chantier mobile qu'une fois le shell web et les points d'entree Unity bien figes.

Options saines:

- Option A
  - garder l'export Unity intact dans `Builds/`
  - creer un vrai site d'accueil autour, a cote, qui embarque ou lance la galerie
- Option B
  - maintenir un `index.html` de deploiement custom hors pipeline Unity
  - le faire pointer vers les fichiers du build
- Option C
  - migrer vers un vrai template WebGL Unity custom
  - utile seulement si l'on veut que chaque rebuild regenere la page finale

Recommendation actuelle:

- `Option A` ou `Option B` est la voie la plus simple et la moins risquee
- `Option C` est plus structurante, a reserver a une demande explicite

## 10. Risques et dettes techniques prioritaires

### 1. Documentation partiellement desynchronisee

Le depot a maintenant une source de verite web claire dans `Web/`, mais certaines notes et traces d'ancien etat parlent encore de `SampleScene` ou de `Assets/Scripts/index.html`.

Risque:

- perte de temps pendant les sessions de travail
- confusion sur la scene active ou le bon point d'entree web
- mauvaises decisions prises sur un etat qui n'est plus celui du repo

### 2. Navigation souris probablement perfectible

Le `PlayerController` normalise `mouseDelta` avant usage.

Risque:

- sensation de controle peu naturelle
- manque de finesse dans la rotation
- experience de visite affaiblie

### 3. Cap global a 30 FPS

`SceneManager` force `Application.targetFrameRate = 30`.

Risque:

- rendu moins fluide sur desktop
- perception de lourdeur meme si le contenu est simple

### 4. Forte dependance Inspector

Une partie importante du comportement repose sur:

- references scene
- triggers
- animators
- canvases
- particle systems

Risque:

- une petite modification scene/prefab peut casser un flux sans erreur compile

### 5. Faible couverture produit hors desktop

Le projet contient des schemes `Touch`/`Gamepad`/`XR`, mais pas de logique gameplay equivalente.

Risque:

- attente implicite d'une compatibilite qui n'est pas reellement fournie

### 6. Pas de tests automatises

Risque:

- regressions silencieuses sur scene/prefab/input

## 11. Forces du projet

- codebase courte et lisible
- architecture simple a reprendre
- scene unique facile a auditer
- interactions locales bien decouplees
- export WebGL deja existant
- intention produit claire
- bon socle pour articulation future avec le projet mobile AR

## 12. Etat local observe pendant l'analyse

Le worktree local n'est pas propre:

- `Assets/Scenes/MainScene.unity` et son `.meta` sont presents mais non suivis
- `ProjectSettings/EditorBuildSettings.asset` a ete modifie pour pointer vers `Assets/Scenes/MainScene.unity`
- `Assets/Scenes/SampleScene.unity` et son `.meta` sont supprimes du worktree
- `Assets/Scripts/index.html` et son `.meta` sont supprimes du worktree
- `Web/` est present comme nouveau dossier de deploiement non suivi
- `Assets/Scripts/WebClientBootstrap.cs` et son `.meta` sont non suivis
- `Assets/Sprites/` est non suivi
- `NOTES.md` est lui-meme modifie

Conseil:

- considerer le depot comme etant en phase de transition structurelle
- eviter toute edition large de scene/prefab sans valider d'abord l'origine des changements deja presents

## 13. Audit serveur OVH du 2026-04-03

Constats verifies sur le serveur de production `venise.kirrril.com`:

- OS: Ubuntu sur noyau `6.11.0-26-generic`
- Apache: `2.4.58 (Ubuntu)`
- vhost du site: `venise.kirrril.com`
- `DocumentRoot`: `/var/www/venise.kirrril.com`
- le serveur prend bien en charge les `.htaccess`
- `AccessFileName` est bien `.htaccess`
- `AllowOverride All` est actif sur le vhost du site

Modules Apache utiles observes:

- `headers_module`
- `mime_module`
- `deflate_module`
- `rewrite_module`
- pas de module Brotli actif

Arborescence de prod observee:

- `/var/www/venise.kirrril.com/index.html`
- `/var/www/venise.kirrril.com/Build/Builds.loader.js`
- `/var/www/venise.kirrril.com/Build/Builds.framework.js`
- `/var/www/venise.kirrril.com/Build/Builds.data`
- `/var/www/venise.kirrril.com/Build/Builds.wasm`

Arborescence cible recommandee pour les prochains deploiements:

```text
/var/www/venise.kirrril.com/
├─ .htaccess
├─ index.html
├─ Build/
│  ├─ Builds.loader.js
│  ├─ Builds.framework.js
│  ├─ Builds.data
│  └─ Builds.wasm
├─ StreamingAssets/      <- a deployer si Unity en genere un
└─ VeniceTargets.pdf
```

Mapping local -> serveur a suivre:

- `Web/index.html` -> `/var/www/venise.kirrril.com/index.html`
- `Web/.htaccess` -> `/var/www/venise.kirrril.com/.htaccess`
- `Builds/Build/*` -> `/var/www/venise.kirrril.com/Build/`
- `Builds/StreamingAssets/*` -> `/var/www/venise.kirrril.com/StreamingAssets/` si ce dossier existe dans un futur build

Convention retenue:

- `Web/index.html` est maintenant la source de verite du site de deploiement
- `Builds/index.html` reste un artefact Unity a ignorer
- l'arborescence serveur actuelle est conservee car elle est deja simple et adaptee au site

Compression et headers observes:

- l'Inspector Unity confirme que `Compression Format` est actuellement sur `Disabled`
- les artefacts locaux observes dans `Builds/Build` sont non precompresses: `.data`, `.framework.js`, `.loader.js`, `.wasm`
- pas de fichiers precompresses `.br` ou `.gz` dans `Build/`
- Apache sert actuellement les assets Unity avec `Content-Encoding: gzip`
- le serveur gere donc aujourd'hui une compression dynamique `gzip`
- aucun support Brotli n'a ete observe
- `Builds.wasm` est servi avec `Content-Type: application/wasm`
- `Builds.framework.js` est servi avec `Content-Type: application/javascript`
- un `Cache-Control: public, max-age=31536000, immutable` est deja observe sur les fichiers de build
- une `Content-Security-Policy` existe deja cote serveur sur le site actuel

Contenu utile du `.htaccess` de prod confirme:

- `SetOutputFilter DEFLATE` est actif
- `AddOutputFilterByType DEFLATE` cible `application/javascript`, `application/wasm`, `application/octet-stream`, `text/html`, `text/plain`, `text/css`
- `AddType` declare bien `.wasm`, `.js` et `.data`
- la CSP actuelle est: `default-src 'self'; script-src 'self' 'unsafe-eval'; style-src 'self' 'unsafe-inline';`
- le cache long actuel cible `.(data|wasm|js)$`
- `index.html` est servi avec `Cache-Control: no-cache, must-revalidate`
- `favicon.ico` est servi avec `Cache-Control: public, max-age=86400`

Implication pour la strategie de deploiement:

- inutile de preparer une configuration `.htaccess` basee sur Brotli
- il faut viser un deploiement compatible `gzip` dynamique Apache
- le futur `.htaccess` doit rester proche de la config actuelle de prod, centre sur:
  - types MIME
  - compression `gzip`
  - cache long sur les fichiers Unity versionnes
  - CSP minimale compatible Unity WebGL

## 14. Hypotheses explicites

- Je pars du principe que le projet mobile AR est volontairement separe et hors scope technique de ce depot.

## 15. Prochaines etapes utiles

- figer la strategie web entre build Unity et site de presentation
- remettre `NOTES.md` et la documentation courte en coherence avec `MainScene` et `Web/`
- auditer puis ameliorer la navigation joueur si souhaite
- definir l'experience d'accueil du site:
  - page hero
  - consignes clavier/souris
  - narration du lien Web gallery <-> mobile AR
- clarifier les cibles de performance WebGL
- preparer la documentation d'integration avec le futur projet mobile

## 16. Fullscreen et cursor lock

Decision d'architecture retenue:

- le fullscreen desktop appartient au shell web, pas au build Unity
- la source de verite de cette logique est `Web/index.html` + `Web/app.js`
- le build Unity reste responsable du gameplay et de l'intention de cursor lock

Conventions desktop retenues:

- les hints d'utilisation du build Unity sont affiches sous forme de hints statiques visibles en permanence sur la version desktop
- ces hints desktop couvrent:
  - deplacement `WASD` et fleches
  - `mouse look`
  - `click to lock cursor`
  - `Esc to unlock cursor`
  - `F to fullscreen`
- ces hints appartiennent a l'experience du build Unity et non au shell HTML
- le shell web reste volontairement leger et n'ajoute pas d'UI de controle redondante pour la desktop

Implementation actuelle du fullscreen desktop:

- le shell web ecoute la touche `F` dans `Web/app.js`
- `F` n'est pas un raccourci natif navigateur: c'est un raccourci custom du projet
- le shell demande le fullscreen sur `#unity-container` via l'API navigateur
- la sortie du fullscreen avec `Esc` est native navigateur
- le message navigateur du type `Pour quitter le plein ecran, appuyez sur Echap` est natif
- l'UI web ne gere plus de bouton fullscreen cliquable
- le comportement souhaite pour la desktop est valide:
  - `F` puis `F` alterne entree / sortie du fullscreen
  - `F` puis `Esc` permet entree puis sortie avec le comportement natif navigateur
- les hints fullscreen vivent dans le canvas Unity, pas dans le shell web

Pourquoi le fullscreen est cote web:

- c'est une responsabilite du navigateur
- cela concerne l'enveloppe de page autour du build
- ce n'est pas une logique propre a la scene 3D

Etat actuel du cursor lock:

- Unity demande actuellement le lock dans `Assets/Scripts/SceneManager.cs`
- `SceneManager.Start()` execute:
  - `Cursor.lockState = CursorLockMode.Locked`
  - `Cursor.visible = false`
- `Assets/Scripts/PlayerController.cs` remet aussi `Cursor.visible = false` pendant le jeu
- le shell web ne gere pas activement le cursor lock

Comportement WebGL a retenir:

- en WebGL, le navigateur garde l'autorite finale sur le pointer lock
- l'appel Unity exprime l'intention de lock, mais le navigateur decide quand il l'accorde reellement
- en pratique, le lancement du build et le vrai lock du curseur ne sont pas toujours consommes par le meme clic
- UX actuelle probable desktop:
  - clic 1: `Enter the gallery` lance le build Unity
  - clic 2 dans la scene: le navigateur peut alors capturer le curseur
- `Esc` sert ensuite a liberer le curseur et/ou sortir du fullscreen selon l'etat navigateur

Lecture architecturale retenue:

- fullscreen: couche web / shell web
- cursor lock: intention cote Unity, mecanique finale arbitree par le navigateur
- cette gestion actuelle est acceptable pour desktop
- sur mobile, les appels Unity de cursor lock sont inutiles mais pas bloquants
- si un nettoyage futur est souhaite, il sera pertinent de ne demander explicitement le cursor lock qu'en mode desktop via `WebClientBootstrap`

Point de deploiement associe:

- la logique active de lancement Unity est maintenant dans `Web/app.js`
- le versioning des assets du shell web est volontairement minimal et manuel
- la version de deploiement est centralisee dans `Web/index.html` via:
  - `<meta name="site-version" content="...">`
  - `Build/Builds.loader.js?v=...`
  - `app.js?v=...`
- cette approche est consideree comme suffisante a ce stade tant que le deploiement reste artisanal et maitrise
- le fond `Assets/Grid2.png` et les autres assets web peuvent etre versionnes a partir de cette meme valeur cote `Web/app.js`

## 17. Etat reel retenu a ce jour

Pour eviter toute ambiguite dans les prochaines sessions, les conventions a retenir sont:

- scene active du build: `Assets/Scenes/MainScene.unity`
- shell web source de verite: `Web/index.html` + `Web/app.js`
- artefact Unity a ignorer comme source: `Builds/index.html`
- fullscreen desktop: gere par le shell web
- hints desktop: affiches en permanence dans le build Unity
- cursor lock: intention cote Unity, decision finale cote navigateur
- versioning shell web: manuel via `meta[name="site-version"]`
- bootstrap web -> Unity: `Assets/Scripts/WebClientBootstrap.cs`
