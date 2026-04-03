# NOTES.md

## Analyse du projet Web gallery

## 1. Resume executif

Le projet est une base Unity WebGL simple, lisible et deja exploitable pour une galerie virtuelle 3D. Il repose sur une seule scene, un seul prefab joueur, quelques objets interactifs declenches par triggers, et un export WebGL deja present dans `Builds/`.

Le coeur de la scene est sain pour un prototype ou une premiere experience publique:

- navigation clavier/souris
- rendu URP
- animations et particules
- UI contextuelle en world space
- build WebGL exporte

En revanche, l'ensemble reste encore au stade "prototype vertical slice" sur plusieurs points:

- le site web autour du build n'est pas stabilise
- il existe deux `index.html` avec des roles differents
- la logique de navigation est rudimentaire
- plusieurs comportements reposent sur l'Inspector
- il n'y a pas de tests automatises

## 2. Cartographie du depot

- `Assets/Scenes/SampleScene.unity`
  - unique scene du build
- `Assets/Prefabs/PlayerPrefab.prefab`
  - prefab du joueur avec `PlayerInput`, `PlayerController`, `Rigidbody`
- `Assets/Prefabs/CanvasOffset.prefab`
  - prefab de canvas monde oriente vers la camera
- `Assets/Scripts/`
  - scripts gameplay + un `index.html` custom de travail
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
- `webGLDataCaching` active
- `webGLThreadsSupport` desactive
- memoire initiale `512`
- memoire max `2048`
- batching `WebGL` desactive dans `PlayerSettings`

Lecture produit:

- le projet cherche davantage une fidelite visuelle simple qu'une optimisation WebGL agressive
- c'est acceptable pour un prototype desktop, moins pour une cible large ou des machines modestes

## 8. Analyse du `index.html`

### Point cle

Il y a deux fichiers `index.html` a ne pas confondre:

- `Builds/index.html`
  - page generee par Unity pour l'export WebGL
- `Assets/Scripts/index.html`
  - vrai fichier HTML source a maintenir pour le site, confirme par l'utilisateur

### Ce que dit la configuration Unity

Le projet est regle sur:

- `webGLTemplate: APPLICATION:Default`

Donc:

- `Assets/Scripts/index.html` n'est pas utilise automatiquement comme template WebGL par la configuration Unity actuelle
- toute regeneration du build continue de produire `Builds/index.html` selon le template par defaut, sauf changement explicite de strategie

Mais, convention de projet confirmee:

- le vrai `index.html` du site a faire evoluer est `Assets/Scripts/index.html`
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

### Analyse de `Assets/Scripts/index.html`

Forces:

- intention claire de faire une page d'accueil plus accueillante
- overlay de demarrage
- CTA `Enter the gallery`
- chargement au clic, utile pour l'audio/pointer lock et l'experience immersive
- design sobre et coherent avec une galerie numerique

Faiblesses:

- le fichier est place dans `Assets/Scripts/`, ce qui est un emplacement ambigu pour une page web de deploiement
- pas de gestion d'erreur si `createUnityInstance` echoue
- pas de bouton fullscreen
- pas de gestion explicite du warning banner Unity
- pas de contenu de presentation autour du canvas
- pas de SEO, pas de meta description, pas de partage social
- pas de strategie claire de versioning/deploiement
- `productName` y vaut `VeniseARShow` alors que le projet Unity est `Venise_WebGallery`, signe de derive nomenclature

Conclusion:

- ce fichier ressemble a une bonne base de maquette ou de wrapper custom
- dans ce projet, il doit etre traite comme la source de verite du HTML du site
- il reste separe du template WebGL Unity par defaut, ce qui implique une discipline de deploiement explicite

## 9. Recommandation pour le futur travail sur le site

Ordre recommande:

1. Clarifier quel `index.html` est reellement deploye sur le VPS OVH.
2. Decider la strategie de source de verite.
3. Ensuite seulement faire evoluer le HTML.

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

### 1. Ambiguite du point d'entree web

Deux `index.html` coexistent avec des roles differents.

Risque:

- modifier le mauvais fichier
- perdre les changements au prochain build
- deployer une page qui n'est pas celle testee localement

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

- modifications non committeees sur `Assets/Scenes/SampleScene.unity`
- modifications non committeees sur `Assets/Prefabs/CanvasOffset.prefab`
- dossier `Assets/Fonts/` non suivi

Conseil:

- eviter toute edition large de scene/prefab sans valider d'abord l'origine de ces changements

## 13. Hypotheses explicites

- Je n'ai pas inspecte le VPS OVH ni la page reellement servie en production.
- L'analyse du site se base sur les fichiers presents dans le depot seulement.
- Je pars du principe que le projet mobile AR est volontairement separe et hors scope technique de ce depot.

## 14. Prochaines etapes utiles

- figer la strategie web entre build Unity et site de presentation
- auditer puis ameliorer la navigation joueur si souhaite
- definir l'experience d'accueil du site:
  - page hero
  - consignes clavier/souris
  - bouton plein ecran
  - narration du lien Web gallery <-> mobile AR
- clarifier les cibles de performance WebGL
- preparer la documentation d'integration avec le futur projet mobile
