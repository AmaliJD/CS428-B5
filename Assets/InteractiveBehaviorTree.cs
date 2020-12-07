using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;

public class InteractiveBehaviorTree : MonoBehaviour
{
	public Transform meetingPoint;
	public GameObject[] people;
	private string[] moods;
	private float timer = 0;

	private BehaviorAgent behaviorAgent;

	// Use this for initialization
	void Start()
	{
		moods = new string[people.Length];

		behaviorAgent = new BehaviorAgent(BuildTreeRoot());
		BehaviorManager.Instance.Register(behaviorAgent);
		behaviorAgent.StartBehavior();
	}
	void Update()
	{
		timer += Time.deltaTime;
	}

	#region Affordances
	protected Node GoTo(GameObject person, Transform target)
	{
		Debug.Log("\t      GoTo");
		Val<Vector3> position = Val.V(() => target.position);

		return new Sequence
		(
			person.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(position, 2f)
		);
	}
	protected Node Wave(GameObject person)
	{
		Debug.Log("\t      Wave");

		return new Sequence
		(
			person.GetComponent<BehaviorMecanim>().ST_PlayHandGesture(Val.V(() => "WAVE"), Val.V(() => (long)10))
		);
	}
	protected Node Dance(GameObject person)
	{
		Debug.Log("\t      Dance");

		return new Sequence
		(
			person.GetComponent<BehaviorMecanim>().Node_BodyAnimation(Val.V(() => "Breakdance"), Val.V(() => true))
		);
	}
	protected Node Suprised(GameObject person, int index)
	{
		Debug.Log("\t      Suprised");
		moods[index] = "suprised";

		return new Sequence
		(
			person.GetComponent<BehaviorMecanim>().Node_BodyAnimation(Val.V(() => "Suprised"), Val.V(() => true))
		);
	}
	protected Node Bored(GameObject person, int index)
	{
		Debug.Log("\t      Bored");
		moods[index] = "bored";

		return new Sequence
		(
			person.GetComponent<BehaviorMecanim>().Node_BodyAnimation(Val.V(() => "Yawn"), Val.V(() => true))
		);
	}
	#endregion

	protected Node CheckArc(int i)
	{
		Debug.Log("CHECK ARC");
		return new Sequence(
			new LeafAssert(() => i == Blackboard.Input.sceneNumber),
			new LeafInvoke(() => {
				Blackboard.StoryArcs.currArc = (Blackboard.StoryArc)i;
			})
		);
	}
	

	protected Node Behavior(int i)
	{
		Node action = new Sequence();

		if (i == 0)
		{
			Debug.Log("\t Executing MEETUP");
			action = new Sequence(
						new SequenceParallel(
							GoTo(people[0], meetingPoint),
							GoTo(people[1], meetingPoint),
							GoTo(people[2], meetingPoint)
						),

						new SequenceParallel(
							Wave(people[0]),
							Wave(people[1]),
							Wave(people[2])
						),

						new LeafWait(1000)
					);
		}
		else if (i == 1)
		{
			Debug.Log("\t Executing DANCE");
			action = new Sequence(
						new ChooseOne(
							Dance(people[0]), Suprised(people[0], 0), Bored(people[0], 0)
						),
						new ChooseOne(
							Dance(people[1]), Suprised(people[1], 1), Bored(people[1], 1)
						),
						new ChooseOne(
							Dance(people[2]), Suprised(people[2], 2), Bored(people[2], 2)
						)
					);
		}

		return action;
	}

	protected Node SelectStory(int i)
	{
		Debug.Log("\t Selecting Story " + i);
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

	#region Story Orginization
	protected Node BuildTreeRoot()
	{
		Node roaming = new DecoratorLoop(
						new Sequence(
								//new LeafInvoke(() => { Debug.Log("LOOPING AGAIN"); }),
								//new DecoratorForceStatus(RunStatus.Success, Input()),
								//new DecoratorForceStatus(RunStatus.Success, MaintainArcs()),
								//new DecoratorForceStatus(RunStatus.Success, Story())
								Behavior(0), Behavior(1)
							)
						);
		return roaming;
	}
	protected Node Input()
	{
		Debug.Log("INPUT PHASE");
		return new Sequence(
			new LeafInvoke(() => {
				if(timer > 90)
                {
					Blackboard.Input.sceneNumber = 3;
					//Debug.Log("\t Scene Number " + 3);
				}
				else if (timer > 60)
                {
					Blackboard.Input.sceneNumber = 2;
					//Debug.Log("\t Scene Number " + 2);
				}
				else if (timer > 20)
				{
					Blackboard.Input.sceneNumber = 1;
					//Debug.Log("\t Scene Number " + 1);
				}
				else if (timer > 0)
				{
					Blackboard.Input.sceneNumber = 0;
					//Debug.Log("\t Scene Number " + 0);
				}
			})
		);
	}
	protected Node MaintainArcs()
	{
		Debug.Log("MAINTAIN ARCS PHASE");
		return new Sequence(
			new Selector(
				CheckArc(0),
				CheckArc(1),
				CheckArc(2),
				CheckArc(3)
			));
			/*new LeafInvoke(() => {
				switch (Blackboard.Input.sceneNumber)
				{
					case 0:
						Blackboard.StoryArcs.currArc = Blackboard.StoryArc.STORY_0;
						break;
					case 1:
						Blackboard.StoryArcs.currArc = Blackboard.StoryArc.STORY_1;
						break;
					case 2:
						Blackboard.StoryArcs.currArc = Blackboard.StoryArc.STORY_2;
						break;
					case 3:
						Blackboard.StoryArcs.currArc = Blackboard.StoryArc.STORY_3;
						break;
				}
			})
		);*/
	}

	protected Node Story()
	{
		Debug.Log("STORY PHASE");
		return new Sequence(new Sequence(
			new LeafInvoke(() => {
				switch (Blackboard.StoryArcs.currArc)
				{
					case Blackboard.StoryArc.STORY_0:
						SelectStory(0);
						break;
					case Blackboard.StoryArc.STORY_1:
						SelectStory(1);
						break;
					case Blackboard.StoryArc.STORY_2:
						SelectStory(2);
						break;
					case Blackboard.StoryArc.STORY_3:
						SelectStory(3);
						break;
				}
			})
		));
	}
    #endregion

    private struct Blackboard
	{
		public struct Input
		{
			public static int sceneNumber = 0;
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
