using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T>
{
    private List<T> heap;
    private Comparison<T> comparison;

    public int Count => heap.Count;

    public PriorityQueue(Comparison<T> comparison)
    {
        this.heap = new List<T>();
        this.comparison = comparison;
    }

    public void Enqueue(T item)
    {
        heap.Add(item);
        int currentIndex = heap.Count - 1;

        while (currentIndex > 0)
        {
            int parentIndex = (currentIndex - 1) / 2;

            if (comparison(heap[currentIndex], heap[parentIndex]) >= 0)
                break;

            // Swap with parent
            T temp = heap[currentIndex];
            heap[currentIndex] = heap[parentIndex];
            heap[parentIndex] = temp;

            currentIndex = parentIndex;
        }
    }

    public T Dequeue()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        T root = heap[0];
        int lastIndex = heap.Count - 1;
        heap[0] = heap[lastIndex];
        heap.RemoveAt(lastIndex);

        int currentIndex = 0;
        while (true)
        {
            int leftChildIndex = currentIndex * 2 + 1;
            int rightChildIndex = currentIndex * 2 + 2;

            int smallestIndex = currentIndex;

            if (leftChildIndex < heap.Count && comparison(heap[leftChildIndex], heap[smallestIndex]) < 0)
                smallestIndex = leftChildIndex;

            if (rightChildIndex < heap.Count && comparison(heap[rightChildIndex], heap[smallestIndex]) < 0)
                smallestIndex = rightChildIndex;

            if (smallestIndex == currentIndex)
                break;

            // Swap with smallest child
            T temp = heap[currentIndex];
            heap[currentIndex] = heap[smallestIndex];
            heap[smallestIndex] = temp;

            currentIndex = smallestIndex;
        }

        return root;
    }
}
