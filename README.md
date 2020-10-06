# Loop Scroll Rect

## v1.05

These scripts help make your ScrollRect `Reusable`, because it will only build cells when needed. If you have a large number of cells in a scroll rect, you absolutely need it! It will save a lot of time loading and draw call, along with memory in use, while still working smoothly.

中文说明请看[这里](http://qiankanglai.me/2015/08/15/LoopScrollRect/)。

## Demo

Demo for Loop Scroll Rect. Each cell knows its own index, and it is able to modify its content/size/color easily.

Also ScrollBar is supported now! It supports both vertical & horizontal directions, back and forth.

![Demo1](Images/demo1.gif)

![Demo2](Images/demo2.gif)

Demo without mask. As you can see, the cells are only instantiated when needed and recycled.

![Demo3](Images/demo3.gif)

**New**: Scroll to Index

![ScrollToIndex](Images/ScrollToIndex.gif)

## Introduction

The original idea comes from @ivomarel's [InfinityScroll](https://github.com/ivomarel/InfinityScroll). After serveral refactorisations, I almost rewrite all the codes:
- Avoid using `sizeDelta` directly since it doesn't always mean size
- Support GridLayout
- Avoid blocking when dragging back
- Take advantage of pool rather than instantiate/destroy every time
- Improve some other details for performance
- Supports reverse direction
- **Supports ScrollBar** (this doesn't work in Infinite mode, and may behavior strange for cells with different size)

Also, I modified [Easy Object Pool](https://www.assetstore.unity3d.com/cn/#!/content/31928) for recycling the cells.

My scripts copies `ScrollRect` from [UGUI](https://bitbucket.org/Unity-Technologies/ui) rather than inherit `ScrollRect` like InfinityScroll. I need to modify some private variants to make dragging smooth. All my codes is wrapped with comments like `==========LoopScrollRect==========`, making maintaining a little easier.

### Infinite Version

If you need scroll infinitely, you can simply set `totalCount` to a negative number.

### Quick Jump

I've implemented a simple version with `Coroutine`. You can use the following API:

    public void SrollToCell(int index, float speed)

Here is a corner case unsolved yet: You can't jump to the last cells which cannot be pulled to the start.

## Example: Loop Vertical Scroll Rect

These steps may be confusing, so you can just open the demo scene and copy & paste :D

You can also remove EasyObjPool and use your pool instead.

- Prepare cell prefabs
    - The cell needs `Layout Element` attached and preferred width/height
    - You should add a script receiving message `void ScrollCellIndex (int idx) `

![ScrollCell](Images/ScrollCell.png)

- Right click in Hierarchy and click **UI/Loop Horizontal Scroll Rect** or **UI/Loop Vertical Scroll Rect**. It is the same for these two in the Component Menu.
    - Init in Start: call Refill cells automatically when Start
    - Prefab Pool: the EasyObjPool gameObject
    - Prefab Pool Name: the corresponding pool in step 1
    - Total Count: How many cells are available (index: 0 ~ TotalCount-1)
    - Threshold: How many additional pixels of content should be prepared before start or after end?
    - ReverseDirection: If you want scroll from bottom or right, you should toggle this
    - Clear Cells: remove existing cells and keep uninitialized
    - Refill Cells: initialize and fill up cells

![LoopVerticalScrollRect](Images/LoopVerticalScrollRect.png)

If you need scroll from top or left, setting content's pivot to 1 and disable ReverseDirection. Otherwise, you should set 0 to pivot and enable ReverseDirection (I have made `VerticalScroll_Reverse` in the demo scene as reference).

I highly suggests you trying these parameters by hand. More details can be found in the demo scene.
