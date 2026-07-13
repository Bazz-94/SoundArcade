# Domain Code Review

Scope: `Source/Domain` — shared contracts, scene management, menu component model, colors, collision service.

## Testing

- [x] 1. **Menu logic untested.** `Menu` (wrap navigation, selection TTS, item press dispatch) and `SceneManager` (enter/exit ordering, exit event, factory-null guard, same-scene no-op) have no unit tests. Standards require all Domain logic unit tested; only `ProximityCollision` is covered.
  A. Add `MenuTests` and `SceneManagerTests` with fake `IInput`/`ITts`/`IRenderer`/`IScene`. (`Models/Menu.cs`, `Services/SceneManager.cs`)

## Encapsulation (DDD)

- [x] 2. **Public setters everywhere on UI components.** `UIComponent.Color`, `MenuItem.TextColor/X/Y/Width/Height/FontSize`, `Menu.MenuStartY/...` all `{ get; set; }`. Standards: private setters + explicit state-transition methods. `Menu.Render` re-assigns item layout and colors every frame from outside.
  A. Make layout/color setters private or internal; pass layout into `MenuItem.Render` (or a `SetLayout` method) instead of mutating properties per frame. (`Models/UIComponent.cs:32`, `Models/MenuItem.cs:13-18`, `Models/Menu.cs:13-18`, `Models/Menu.cs:113-121`)

- [x] 3. **Menu exposes PAL dependencies publicly.** `Input`, `Tts`, `Renderer` are public get properties; only `Menu` and subclasses need them.
  A. Make `protected` (subclasses like `SettingsMenu` use `Input`) or private. (`Models/Menu.cs:27-29`)

- [x] 4. **Theme/ColorPalette are mutable records with public fields.** `ColorPalette` uses public fields (`Background`, `Accent`, ...), `Theme.FontSize` has public setter, `Theme.ColorPalette` is a public readonly field. Records with mutable fields lose value semantics; fields violate property convention.
  A. Convert to get-only properties (init or constructor). (`Colors/Theme.cs:5-18`)

- [x] 5. **`MenuItem.OnPressed` has `private set` but is never reassigned.**
  A. Make get-only. (`Models/MenuItem.cs:12`)

- [x] 6. **`SceneManager.SceneFactory` is a public mutable property.** Settable at any time; likely property-injected to break a DI cycle, but nothing prevents mid-game reassignment.
  A. If cycle allows, constructor-inject; otherwise add one-time-set guard or document why. (`Services/SceneManager.cs:16`)

## Validation

- [x] 7. **`ControlItem` does not validate `initialValue` or `SetValue` against `Values`.** A value outside the cycle list renders fine but `Cycle` (Application) snaps to nearest — silent drift. Doc says the item "cycles through a fixed set of levels" yet cycling logic lives in Application `SettingsMenu`, not here.
  A. Validate value membership (or clamp) in constructor and `SetValue`; fix doc to say cycling is caller-driven. (`Models/ControlItem.cs:52-54`, `Models/ControlItem.cs:63-67`)

- [x] 8. **`Menu` constructor copies `items` before validating, no null checks on PAL args.** Also validates `Items` after first assignment — reorder so guards run first.
  A. Guard `input`/`tts`/`renderer`/`items` null, validate count before assignments. (`Models/Menu.cs:39-54`)

## Dead / questionable code

- [x] 9. **`IPlayer` is an empty marker interface with no doc comment.** Only `Domain.RiverRun` `Player` implements it; nothing consumes it polymorphically.
  A. Delete, or document intended contract and add members when real need appears. (`Models/IPlayer.cs`)

- [x] 10. **`Menu.WrapIndex` `length <= 0` branch unreachable.** Constructor guarantees at least one item.
  A. Remove branch or keep as defensive guard — minor. (`Models/Menu.cs:88-91`)

## Naming / docs / consistency

- [x] 11. **Missing XML docs.** Standards require concise docs on all classes/methods/properties. Missing: `Theme`, `ColorPalette`, `ColorsHex`, `MenuType`, `IPlayer`, `Menu.SelectFirstItem`, most `Menu`/`MenuItem` public properties, `UIComponent.Color`. Constructor docs stale: `UIComponent` ctor doc omits `color` param; `MenuItem` ctor doc omits `theme` param.
  A. Add/fix doc comments. (multiple files)

- [x] 12. **File/type name mismatch.** `Enums/Menus.cs` contains `MenuType`.
  A. Rename file to `MenuType.cs`. (`Enums/Menus.cs`)

- [x] 13. **Magic layout numbers as property initializers.** `MenuStartY = 350`, `MenuTitleOffsetY = 80`, spacing/width/height/font values hardcoded inline; `Menu.Render` computes title font as `MenuItemFontSize + 4`. Standards: define constants for meaningful values.
  A. Move defaults to named constants (e.g. `MenuLayout` static class) and name the `+ 4` offset. (`Models/Menu.cs:13-18`, `Models/Menu.cs:110`)

- [x] 14. **Inconsistent construction style in `ColorPalette`.** `Background = new Color(...)` vs target-typed `new(...)` for the rest.
  A. Pick one style. (`Colors/Theme.cs:13-17`)
