using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;

public class InteractiveBehaviorTree : MonoBehaviour
{
	public Transform meetingPoint;
	public GameObject[] people;
	private string[] moods;

	//private

	private BehaviorAgent behaviorAgent;

	// Use this for initialization
	void Start()
	{
		behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
		BehaviorManager.Instance.Register(behaviorAgent);
		behaviorAgent.StartBehavior();

		moods = new string[people.Length];
	}

	protected Node ST_ApproachAndWait(Transform target)
	{
		Val<Vector3> position = Val.V(() => target.position);
		return new Sequence(
			new LeafInvoke(() => {
				foreach(GameObject person in people)
				{
					person.GetComponent<BehaviorMecanim>().Node_GoTo(position);
				}
			}),
			new LeafWait(100)
		);
	}

	protected Node BuildTreeRoot()
	{
		Node roaming = new Sequence(
						new Sequence(
								Input(),
								MaintainArcs(),
								Story()
							)
						);
		return roaming;
	}

	protected Node Input()
	{
		return new DecoratorLoop(
			new LeafInvoke(() => {
				for (int i = 0; i <= 3; i++)
				{
					/*if (UserMoveScript.boxes.Contains(bounds[i]))
					{
						Blackboard.UserInput.sceneNumber = i;

						return;
					}*/
					Blackboard.UserInput.sceneNumber = 0;
				}
			})
		);
	}

	protected Node CheckArc(int i)
	{
		return new Sequence(
			new LeafAssert(() => i == Blackboard.UserInput.sceneNumber),
			new LeafInvoke(() => {
				Blackboard.StoryArcs.currArc = (Blackboard.StoryArc)i;
			})
		);
	}
	protected Node MaintainArcs()
	{
		return new DecoratorLoop(
			//new Selector(
			//    CheckArc(-1),
			//    CheckArc(0),
			//    CheckArc(1),
			//    CheckArc(2)
			//)
			new LeafInvoke(() => {
				switch (Blackboard.UserInput.sceneNumber)
				{
					case -1:
						Blackboard.StoryArcs.currArc = Blackboard.StoryArc.STORY_0;
						break;
					case 0:
						Blackboard.StoryArcs.currArc = Blackboard.StoryArc.STORY_1;
						break;
					case 1:
						Blackboard.StoryArcs.currArc = Blackboard.StoryArc.STORY_2;
						break;
					case 2:
						Blackboard.StoryArcs.currArc = Blackboard.StoryArc.STORY_3;
						break;
				}
			})
		);
	}

	protected Node Behavior(int i)
	{
		//if(i == 0)
        //{
			Node action = new DecoratorLoop(
						new SequenceParallel(
							ST_ApproachAndWait(meetingPoint)
						)) ;

			return action;
		//}
	}

	protected Node SelectStory(int i)
	{
		return new SelectorParallel(
			new DecoratorInvert(new DecoratorLoop(new Sequence(
				new LeafAssert(() => Blackboard.StoryArcs.currArc == (Blackboard.StoryArc)Enum.Parse(typeof(Blackboard.StoryArc), "STORY_" + i))
			))),
			new Sequence(
				Behavior(i),
				new DecoratorLoop(new LeafWait(1))
			)
		);
	}

	protected Node Story()
	{
		return new Sequence(
					SelectStory(0),
					SelectStory(1),
					SelectStory(2),
					SelectStory(3)
		);
	}

	private struct Blackboard
	{
		public struct UserInput
		{
			public static int sceneNumber = -1;
		}

		public struct StoryArcs
		{
			public static StoryArc currArc = StoryArc.STORY_0;
		}

		public enum StoryArc
		{
			STORY_0 = 0,	// MEETUP
			STORY_1 = 1,    // DANCE
			STORY_2 = 2,    // REACTION
			STORY_3 = 3,	// RESOLUTION
		}
	}
}
