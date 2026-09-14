# Changelog

## v0.1.5
* `stf.bone`: added `tr` & `tr_armature` properties. They store transforms relative to the parent and armature respectively. `translation` and `rotation` are deprecated.
* `stf.instance.armature`: Deprecated animation path part `component_mods`, replaced with `components`.
* AVA UNIVRM0 detection uses assembly version define.
* Started implementing a VRM1 context.

## v0.1.4
* Added application context import settings.
* Added VRChat context import option to place all physics components separately under a "Physics" GameObject.
* Moved resource import settings from their own interface into their processors.
* Code documentation improvements.

## v0.1.3
* Implemented minor binary format adaptation.

## v0.1.2
* Adapted ava - Resonite avatar context.
* Implemented com.squirrelbite.avatar_setup grab toggle.
* Implemented `stfexp.node.ethereal`.

## v0.1.1
* Bugfix for importing materials with empty images.
* Bugfix for importing nodes with parent bindings.
