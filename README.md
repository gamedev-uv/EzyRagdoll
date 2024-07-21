![](README/header.png)

# EzyRagdoll [![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)](https://github.com/gamedev-uv/EzyRagdoll/blob/main/LICENSE)
A small package which makes adding ragdolls to your game easier!

# 💿 Installation
:warning: This package requires the [**EzyInspector**](https://github.com/gamedev-uv/EzyInspector) & [**EzyReflection**](https://github.com/gamedev-uv/EzyReflection) package in order to function. Make sure you install that package before installing this one.

Through the [**Unity Package Manager**](https://docs.unity3d.com/Manual/upm-ui-giturl.html) using the following Git URLs:
```
https://github.com/gamedev-uv/EzyReflection.git
```

```
https://github.com/gamedev-uv/EzyInspector.git
```

```
https://github.com/gamedev-uv/EzyRagdoll.git
```


# Dependencies 
 - [**EzyInspector**](https://github.com/gamedev-uv/EzyInspector)
 - [**EzyReflection**](https://github.com/gamedev-uv/EzyReflection)

# Overview
## Ragdoll.cs

## Data Members

| Member | Type | Description |
|--------|------|-------------|
| `DisableRagdollAtStart` | `bool` | Whether the ragdoll is to be turned off at the start. |
| `ControlColliders` | `bool` | Whether the colliders are controlled by the ragdoll or not. |
| `ChildrenBodies` | `Rigidbody[]` | All the rigidbodies on the children under this object. |
| `ChildrenColliders` | `Collider[]` | All the colliders on the children under this object. |
| `OnRagdollEnabled` | `UnityEvent` | Event which is called when the ragdoll is enabled. |
| `OnRagdollDisabled` | `UnityEvent` | Event which is called when the ragdoll is disabled. |

## Member Methods 

### `FindReferences`

- `public void FindReferences()`
Finds all the rigidbodies and colliders in the children of the GameObject. It stores them in `ChildrenBodies` and `ChildrenColliders`.

### `SetRagdoll`

- `public void SetRagdoll(bool activationState)`
Sets the activation state of the ragdoll. Enables or disables the rigidbodies and colliders based on the `activationState`.
    - `activationState` (`bool`): The desired activation state of the ragdoll (`true` to enable, `false` to disable).

### `EnableRagdoll`

- `public void EnableRagdoll()`
Activates the ragdoll and invokes the `OnRagdollEnabled` event.

### `DisableRagdoll`

- `public void DisableRagdoll()`
Deactivates the ragdoll and invokes the `OnRagdollDisabled` event.

### `AddForce`

- `public void AddForce(float force)`
Activates the ragdoll and adds an explosive force to it at the object's position.
  - `force` (`float`): The magnitude of the force to be added.

- `public void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Impulse)`
Activates the ragdoll and adds a specified force to it.
    - `force` (`Vector3`): The force to be applied.
    - `forceMode` (`ForceMode`, optional): The mode to use when applying the force (default is `ForceMode.Impulse`).

- `public void AddForce(float force, Vector3 forcePoint, ForceMode forceMode = ForceMode.Impulse)`
Activates the ragdoll and adds an explosive force to it at the specified point.
    - `force` (`float`): The magnitude of the force to be applied.
    - `forcePoint` (`Vector3`): The point at which the force will be applied.
    - `forceMode` (`ForceMode`, optional): The mode to use when applying the force (default is `ForceMode.Impulse`).



## Usage Example

```csharp
using UnityEngine;
using UV.EzyRagdoll;
using System.Collections;

public class RagdollExample : MonoBehaviour
{
    [SerializeField] private Ragdoll _ragdoll;

    IEnumerator Start()
    {
        // Find and initialize references to the child components
        _ragdoll.FindReferences();

        // Enable the ragdoll
        _ragdoll.EnableRagdoll();

        // Add a force to the ragdoll
        _ragdoll.AddForce(100f);

        // Disable the ragdoll after 5 seconds
        yield return new WaitForSeconds(5);
        _ragdoll.DisableRagdoll();
    }
}
