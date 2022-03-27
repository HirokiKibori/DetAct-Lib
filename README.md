# DetAct-Lib
```
Currently, this is a prototype! There will be changes in the future. (Finishing v0.5 is planed in 2022.)
```

## Description
DetAct-Lib is a framework which offers the possibility to define behaviour of an instance with a tree-based behaviour-model. Such a definition is described in a syntactic modelling language (BTML). Currently, a behaviour-model can be used to create a behaviour-tree.\
This framework contains an interpreter to process behaviour-models and an additional set of basic classes (node-types) to create behaviour-trees. The interpreter can be extended with own node-types to create a behaviour-tree without compiling a new parser.

## Used credentials
About behaviour-trees:
- Damian Isla | March 2005\
  **GDC 2005 Proceeding: Handling Complexity in the Halo 2 AI**\
  <https://www.gamasutra.com/view/feature/130663/gdc_2005_proceeding_handling_.php>

- Alex J Champandard and Philip Dunstan | September 2013\
  **The Behavior Tree Starter Kit**\
  *Game AI Pro, pages 73–91 ISBN-13: 978-1466565968*\
  <http://www.gameaipro.com/GameAIPro/GameAIPro_Chapter06_The_Behavior_Tree_Starter_Kit.pdf>

- Ramiro Agis, Sebastian Gottifredi, and Alejandro Garcia | April 2020\
  **An event-driven behavior trees extension to facilitate non-player multi-agent coordination in video games**\
  *Expert Systems with Applications, 155:113457, doi:10.1016/j.eswa.2020.113457*\
  <https://www.researchgate.net/publication/340870872_An_event-driven_behavior_trees_extension_to_facilitate_non-player_multi-agent_coordination_in_video_games>

- Chris Simpson | July 2014\
  **Behavior trees for AI: How they work**\
  <https://www.gamasutra.com/blogs/ChrisSimpson/20140717/221339/Behavior_trees_for_AI_How_they_work.php>

DSL for behaviour-trees:
- Miguel Oliveira, Pedro Mimoso Silva, Pedro Moura, José João Almeida, and Pedro Rangel Henriques | Dagstuhl, Germany, 2020. Schloss Dagstuhl–Leibniz-Zentrum für Informatik\
  **BhTSL, Behavior Trees Specification and Processing**\
  *In Alberto Simões, Pedro Rangel Henriques, and Ricardo Queirós, editors, 9th Symposium on Languages, Applications and Technologies (SLATE 2020), volume 83 of OpenAccess Series in Informatics (OASIcs), pages 4:1–4:13, ISSN: 2190-6807, doi:10.4230/OASIcs.SLATE.2020.4*\
  <https://drops.dagstuhl.de/opus/volltexte/2020/13017>

## Example
There is an example-application for this framework. You can find it here:\
[DetAct - https://github.com/HirokiKibori/DetAct](https://github.com/HirokiKibori/DetAct)

(*On wiki-pages tere will be more examples in the future.*)

## Roadmap
There are many things to do until v0.5:
- add issues
- rename BTML (Behaviour-Tree Modelling Language) to TBML (Tree-based Behaviour Modelling Language)
- implement a new interpreter
  - add/overwrite node-types with C#-attributes
  - handle model-configurations
- refractor node-types to build event driven behaviour-trees
- refractor black-boards (black-board key-list -> set values at start and not in model)
- improve BTML
  - add interrupt-handling
  - add definition of memory-nodes
  - add more types for functions
  - fix bugs (i.e. node-type-parameters)
  - maybe remove the function-list (use C#-attributes instead)
- add a documentation
- create a nuget-package