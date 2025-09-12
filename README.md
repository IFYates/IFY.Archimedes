<div align="center">

![Archimedes](archimedes-256.png)

</div>

# Archimedes
A generator for layered architectural diagrams.

# Overview
Archimedes is a tool designed to help developers create layered architectural diagrams for their software projects. It allows you to define layers and components in a structured way, making it easier to visualise and communicate the architecture of your applications.

# Features
- Define layers and components using a simple configuration file.
- Generate [Mermaid diagrams](https://mermaid.js.org) from the defined architecture.
- Visualize the relationships between different components and layers, drilling down into sub-components as needed.

# Command-line
The dotnet command line tool accepts the following arguments:
> `IFY.Archimedes.exe [ <path> | -file <path> ] [ -dir TD|LR ]`

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
  style: (string, optional) Mermaid-support CSS: https://mermaid.js.org/syntax/flowchart.html#styling-a-node
  expand: (boolean, default: false) Whether the component is always expanded
  children: # (optional) Dictionary of child components
    ChildComponentID: {} # Same structure as parent
  links: # (optional) Dictionary of links to other components as string or object
    TargetComponentID: # Target of the link with just a style
      - default # Can be omitted using '{}'
      - dots
      - line # No arrow
      - thick
      - invisible # Nothing visible
    TargetComponentID2: # Target of the link with full options
      type: # (string, optional) Style of the link
        - default
        - dots
        - line # No arrow
        - thick
        - invisible # Nothing visible
      text: (string, optional) Text for the link
      reverse: (boolean, default: false) Whether the link is reversed (target to source)
```

# Example
Here is an example of a definition file in YAML format:
```yaml
File:
  title: Input file
  type: roof
  detail: JSON or YAML
  links:
    Parser: {}
Archimedes:
  doc: https://github.com/IFYates/IFY.Archimedes
  children:
    Parser:
      type: leanr
      links:
        Structure: {}
        ParserFail: dots
    ParserFail:
      title: Fail
      type: circle2
    Structure:
      title: Hierarchy builder
      links:
        Writer: {}
    Writer:
      type: leanl
      links:
        Output: dots
Output:
  detail: Mermaid diagrams
  type: boat
  links:
    Next: dots
Next:
  title: Your processes
  style: fill:#888, stroke-dasharray:5 5
```

Using command `IFY.Archimedes.exe -file example.yaml`, you would get the following Mermaid diagrams:

<span id="root-d"></span>
> ## All Components
> 
> :::mermaid
> graph LR
> %% All Components
>     File[/"Input file<br><em>JSON or YAML</em>"\]
>     Archimedes[["<a href='#d-archimedes' title='Expand node'>Archimedes</a><br><small><a href='https://github.com/IFYates/IFY.Archimedes' title='Go to documentation'>📖 Documentation</a></small><br><small>4 children</small>"]]
>     Output[\"Output<br><em>Mermaid diagrams</em>"/]
>     Next["Your processes"]
>     style Next fill:#888, stroke-dasharray:5 5
> 
>     File -->|"Via args<br><small><em>To <a href='#d-archimedes'>Parser</a></em></small>"| Archimedes
>     Archimedes -.->|"<small><em>From <a href='#d-archimedes'>Writer</a></em></small>"| Output
>     Output -.-> Next
> :::
> 
> <span id="d-archimedes"></span>
> ## Archimedes
> 
> :::mermaid
> graph LR
> %% Archimedes
>     B638932708823095386(["<small><a href='#root-d'>Back</a></small>"])
> 
>     subgraph Archimedes["<big><strong>Archimedes</strong></big> <sup><a href='https://github.com/IFYates/IFY.Archimedes' title='Go to documentation'>📖</a></sup>"]
>         Parser[/"Parser"/]
>         ParserFail((("Fail")))
>         Structure["Hierarchy builder"]
>         Writer[\"Writer"\]
>     end
>     File[/"Input file<br><em>JSON or YAML</em>"\]
>     Output[\"Output<br><em>Mermaid diagrams</em>"/]
> 
>     Parser --> Structure
>     Parser -.-> ParserFail
>     Structure --> Writer
>     Writer -.-> Output
>     File -->|"Via args"| Parser
> :::