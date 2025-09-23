# Archimedes schema file
[View source schema](Archimedes.yaml)

<span id="root-d"></span>
## All Components

:::mermaid
graph LR
%% All Components
    subgraph ArchFile["Schema file <sup><span title='YAML or JSON(C)'>ℹ️</span></sup>"]
        Includes[".include<br><em>string or string array</em>"]
        style Includes stroke-dasharray: 7
        Config[["<a href='#d-config' title='Expand node'>.config</a><br><small>3 children</small>"]]
        style Config stroke-dasharray: 7
        _Components[["<a href='#d-_components' title='Expand node'>{component ID}</a><br><em>Dictionary entry</em><br><small>7 children</small>"]]
    end

    _Components -.-> Config
:::

<span id="d-config"></span>
## .config

:::mermaid
graph LR
%% .config
    B638935522411036371(["<small><a href='#root-d'>Back</a></small>"])

    subgraph ArchFile["Schema file <sup><span title='YAML or JSON(C)'>ℹ️</span></sup>"]
        subgraph Config["<big><strong>.config</strong></big>"]
            config_title["title<br><em>string</em>"]
            style config_title stroke-dasharray: 7
            direction["direction<br><em>LR (default) or TD</em>"]
            style direction stroke-dasharray: 7
            nodeTypes[["<a href='#d-nodetypes' title='Expand node'>nodeTypes</a><br><em>Dictionary</em><br><small>6 children</small>"]]
            style nodeTypes stroke-dasharray: 7
        end
        _Components[["<a href='#d-_components' title='Expand node'>{component ID}</a><br><em>Dictionary entry</em><br><small>7 children</small>"]]
    end

    _Components -.->|"<small><em>From <a href='#d-_components'>style</a></em></small>"| nodeTypes
:::

<span id="d-nodetypes"></span>
## <a href='#d-config'>.config</a> / nodeTypes

:::mermaid
graph LR
%% nodeTypes
    B638935522411111715(["<small><a href='#d-config'>Back</a></small>"])

    subgraph ArchFile["Schema file <sup><span title='YAML or JSON(C)'>ℹ️</span></sup>"]
        subgraph Config[".config"]
            subgraph nodeTypes["<big><strong>nodeTypes</strong></big> <sup><span title='Dictionary'>ℹ️</span></sup>"]
                base["base<br><em>Base style name</em>"]
                style base stroke-dasharray: 7
                fill["fill<br><em>CSS colour</em>"]
                style fill stroke-dasharray: 7
                color["color<br><em>CSS colour</em>"]
                style color stroke-dasharray: 7
                stroke["stroke<br><em>HTML colour</em>"]
                style stroke stroke-dasharray: 7
                strokeDashArray["stroke-dashArray<br><em>CSS border dash array</em>"]
                style strokeDashArray stroke-dasharray: 7
                strokeWidth["stroke-width<br><em>CSS width</em>"]
                style strokeWidth stroke-dasharray: 7
            end
        end
        _Components[["<a href='#d-_components' title='Expand node'>{component ID}</a><br><em>Dictionary entry</em><br><small>7 children</small>"]]
    end

    _Components -.->|"<small><em>From <a href='#d-_components'>style</a></em></small>"| nodeTypes
:::

<span id="d-_components"></span>
## <a href='#d-config'>.config</a> / <a href='#d-nodetypes'>nodeTypes</a> / {component ID}

:::mermaid
graph LR
%% {component ID}
    B638935522411116171(["<small><a href='#d-nodetypes'>Back</a></small>"])

    subgraph ArchFile["Schema file <sup><span title='YAML or JSON(C)'>ℹ️</span></sup>"]
        subgraph _Components["<big><strong>{component ID}</strong></big> <sup><span title='Dictionary entry'>ℹ️</span></sup>"]
            node_title["title<br><em>string (defaults to ID)</em>"]
            style node_title stroke-dasharray: 7
            node_style["style<br><em>Any style name (defaults to #39;node#39; or #39;block#39;)</em>"]
            style node_style stroke-dasharray: 7
            detail["detail<br><em>string</em>"]
            style detail stroke-dasharray: 7
            doc["doc<br><em>URL</em>"]
            style doc stroke-dasharray: 7
            expand["expand<br><em>boolean</em>"]
            style expand stroke-dasharray: 7
            children["children<br><em>Dictionary of components</em>"]
            style children stroke-dasharray: 7
            links[["<a href='#d-links' title='Expand node'>links</a><br><em>Dictionary</em><br><small>1 child</small>"]]
            style links stroke-dasharray: 7
        end
        Config[["<a href='#d-config' title='Expand node'>.config</a><br><small>3 children</small>"]]
        style Config stroke-dasharray: 7
    end

    node_style -.->|"<small><em>To <a href='#d-config'>nodeTypes</a></em></small>"| Config
:::

<span id="d-links"></span>
## <a href='#d-config'>.config</a> / <a href='#d-nodetypes'>nodeTypes</a> / <a href='#d-_components'>{component ID}</a> / links

:::mermaid
graph LR
%% links
    B638935522411116906(["<small><a href='#d-_components'>Back</a></small>"])

    subgraph ArchFile["Schema file <sup><span title='YAML or JSON(C)'>ℹ️</span></sup>"]
        subgraph _Components["{component ID} <sup><span title='Dictionary entry'>ℹ️</span></sup>"]
            subgraph links["<big><strong>links</strong></big> <sup><span title='Dictionary'>ℹ️</span></sup>"]
                _Links[["<a href='#d-_links' title='Expand node'>{target component ID}</a><br><em>Dictionary entry 
 string (link style) or object</em><br><small>3 children</small>"]]
                style _Links stroke-dasharray: 7
            end
        end
    end
:::

<span id="d-_links"></span>
## <a href='#d-config'>.config</a> / <a href='#d-nodetypes'>nodeTypes</a> / <a href='#d-_components'>{component ID}</a> / <a href='#d-links'>links</a> / {target component ID}

:::mermaid
graph LR
%% {target component ID}
    B638935522411117155(["<small><a href='#d-links'>Back</a></small>"])

    subgraph ArchFile["Schema file <sup><span title='YAML or JSON(C)'>ℹ️</span></sup>"]
        subgraph _Components["{component ID} <sup><span title='Dictionary entry'>ℹ️</span></sup>"]
            subgraph links["links <sup><span title='Dictionary'>ℹ️</span></sup>"]
                subgraph _Links["<big><strong>{target component ID}</strong></big> <sup><span title='Dictionary entry 
 string (link style) or object'>ℹ️</span></sup>"]
                    link_style["style<br><em>Any style name (defaults to #39;default#39;)</em>"]
                    style link_style stroke-dasharray: 7
                    text["text<br><em>string</em>"]
                    style text stroke-dasharray: 7
                    reverse["reverse<br><em>boolean</em>"]
                    style reverse stroke-dasharray: 7
                end
            end
        end
    end
:::
