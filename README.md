# Archimedes
A generator for layered architectural diagrams.

# Overview
Archimedes is a tool designed to help developers create layered architectural diagrams for their software projects. It allows you to define layers and components in a structured way, making it easier to visualise and communicate the architecture of your applications.

# Features
- Define layers and components using a simple configuration file.
- Generate Mermaid diagrams from the defined architecture.
- Visualize the relationships between different components and layers, drilling down into sub-components as needed.

# Definition file
The architecture is defined as either JSON (comments allowed) or YAML.

The structure of the file is as follows:
```yaml
# Root is dictionary of components
ComponentID:
  type: # (optional) Display type of component
    - node
    - block
    - data
    - pill
    - soft
    - circle
    - circle2
    - flag
    - diamond
    - hexagon
    - leanR
    - leanL
    - roof
    - boat
  title: (string, optional, default same as ID) Title of the component
  detail: (string, optional) Additional details about the component
  doc: (string, optional) URL to documentation
  expand: (boolean, default: false) Whether the component is always expanded
  children: # (optional) Dictionary of child components
    ChildComponentID: {} # Same structure as parent
  links: # (optional) Dictionary of links to other components as string or object
    TargetComponentID: # Style of the link
      - solid
      - dashed
      - thick
      - dotted
    TargetComponentID2:
      type:
        - solid
        - dashed
        - thick
        - dotted
      text: Text for the link
```