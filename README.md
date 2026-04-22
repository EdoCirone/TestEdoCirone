# Inventory System Test

Sistema inventario realizzato in Unity con:

- raccolta oggetti dal mondo
- visualizzazione a slot fissi
- utilizzo item con effetti diversi
- drag and drop tra slot
- drop degli oggetti nel mondo
- HUD aggiornato tramite eventi
- animazione di apertura/chiusura inventario
- warning UI per inventario pieno

---

## Funzionalità implementate

### Inventario
- Inventario a capacità fissa
- Slot vuoti rappresentati con `null` per mantenere mapping stabile tra indice e UI
- Aggiunta item al primo slot libero disponibile
- Rimozione item senza modificare la dimensione della lista
- Spostamento e scambio item tra slot

### Raccolta item
- Raccolta tramite click sugli oggetti nel mondo
- Uso di `OnMouseDown()` su `ItemPickerControl` (soluzione di prototipo)
- Se inventario pieno, item non raccolto e viene mostrato un warning UI

### Uso item
Gli item usano `ScriptableObject` per definire l’effetto sul player.

Effetti implementati:
- Heal
- Damage
- Speed
- Strength

### Drag and drop
- Drag tra slot
- Swap automatico se il target è occupato
- Drop nel mondo se rilasciato fuori dal pannello inventario
- Preview icona creata a runtime con `Image` separata

### HUD
- Barra salute
- Testo statistiche
- Aggiornamento tramite eventi (`OnHealthChanged`, `OnStatsChanged`)

### UI e animazioni
- Apertura pannello inventario animata
- Comparsa titolo
- Apparizione slot in cascata
- Chiusura animata
- Warning inventario pieno su canvas separato

### UI Panels (event-driven feedback)
Sistema centralizzato per la gestione dei pannelli UI:

- Inventory Full
- Health Already Full
- Player Death

Gestiti tramite:
- `UIPanelManager`
- `PanelAnimation`

I pannelli reagiscono a eventi di gameplay esposti dai sistemi core:
- `OnInventoryFull`
- `OnHealthAlreadyFull`
- `OnPlayerDeath`

Il manager si sottoscrive agli eventi e apre il pannello corretto, chiudendo gli altri.

---

## Controlli

- Click sinistro su item → raccoglie
- Click su slot → usa item
- Tasto `I` → apre/chiude inventario

Drag:
- su altro slot → move/swap
- fuori inventario → drop nel mondo

- Click fuori dal warning → chiude warning
- Restart button → ricarica scena

Nota: `ExitGame()` funziona solo in build.

---

## Architettura

### Manager e logica
- **InventoryManager** → gestione inventario
- **PlayerStats** → salute e statistiche
- **GameManager** → gestione scena

### Item e dati
- **ItemData** → dati item
- **AbstractEffectData** → base effetti
- **Heal / Damage / Speed / Strength** → implementazioni

### Interazione mondo
- **ItemPickerControl** → rappresenta item nel mondo
- **PlayerItemCollector** → raccolta item
- **PlayerItemDropper** → drop nel mondo

### UI
- **InventoryUI** → gestione UI inventario
- **InventorySlotUI** → slot + drag & drop
- **HUDManager** → aggiornamento HUD
- **ResponsiveGrid** → layout dinamico
- **InventoryAnimation** → animazioni
  
---

### Audio System (event-based)

Sistema audio basato su event bus:

- `AudioEvents` → invio eventi globali
- `AudioManager` → gestione e riproduzione audio

Due tipi di trigger:
- AudioCue (enum) per suoni globali (UI, pickup, drop, ecc.)
- AudioClip diretto per effetti specifici associati agli item (tramite ScriptableObject)
  
Separazione tra:
- Audio UI (`AudioSource` dedicata)
- Audio gameplay (`AudioSource` dedicata)

I suoni vengono triggerati in corrispondenza delle azioni (es. apertura inventario, uso item, pickup, ecc.).

---

## Scelte implementative

### Inventario non singleton
Permette più inventari (player, NPC, ecc).

### Slot fissi con `null`
- mapping diretto con UI
- griglia stabile
- logica semplificata

### Effetti con ScriptableObject
Aggiungere un effetto non richiede modifiche al sistema esistente.

### Drag preview separata
Evita problemi di scala e gerarchia UI.

---

## Problemi e soluzioni

### 1. Preview drag scalata male
**Soluzione:** creata nuova `Image` dedicata.

### 2. Warning non sopra la UI
**Soluzione:** canvas separato con sorting più alto.

### 3. Chiusura warning
**Soluzione:** area cliccabile (`CloseArea`).

### 4. HUD aggiornato ogni frame
**Soluzione:** uso eventi.

### 5. Animazioni poco leggibili
**Soluzione:** sequenza DOTween strutturata.

---

## Limiti attuali

- Raccolta con `OnMouseDown()` (prototipo)
- Drag usa reference statica (non scalabile)
- No stack item
- No tipologie avanzate di item
- Drop richiede raycast valido
- AudioMixer non implementato (gestione volumi base tramite AudioSource)

---

## Setup scena

- InventoryManager collegato a PlayerStats
- Player con:
  - PlayerStats
  - PlayerItemCollector
  - PlayerItemDropper
- UIManager con:
  - HUDManager
  - InventoryUI
  - InventoryAnimation
- Grid con ResponsiveGrid
- WarningPanels su canvas separato
- EventSystem
- Main Camera
- Collider nel mondo
- Prefab assegnati agli ItemData

---

## Struttura progetto

### Script
- Scripts/Managers
- Scripts/Player
- Scripts/UI

### Dati
- Data/Items
- Data/Effects

### Prefab
- Prefabs/Items
- Prefabs/UI

---

## Estensioni future

- inventari multipli
- stack item
- tooltip
- ordinamento
- input system centralizzato
- miglior interazione mondo
- audio feedback
- animazioni UI aggiuntive

---

## Build testata

Testato in editor e build Windows con:

- raccolta item
- warning inventario pieno
- uso item
- drag and drop
- drop nel mondo
- aggiornamento HUD
- apertura/chiusura inventario
- restart scena
