using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;
using System.Collections.Generic;

public class InteractiveBehaviorTree : MonoBehaviour
{
	public Transform meetingPoint;
	public GameObject[] people;
	//private string[] moods;
	//private List<string> moodList;

	private string mood0 = "";
	private string mood1 = "";
	private string mood2 = "";

	private float timer = 0;
	int alreadyInAction = -1;

	private BehaviorAgent behaviorAgent;

	// Use this for initialization
	void Start()
	{
		//moods = new string[people.Length];

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
			person.GetComponent<BehaviorMecanim>().Node_OrientTowards(position),
			person.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(position, 2.5f)
		);
	}
	protected Node Wave(GameObject person)
	{
		//Debug.Log("\t      Wave");
		//person.GetComponent<BodyMecanim>().ResetAnimation();

		return new Sequence
		(
			//person.GetComponent<BehaviorMecanim>().Node_BodyAnimation(Val.V(() => "Idle"), Val.V(() => false)),
			new LeafInvoke(() =>
			{
				Debug.Log("\t      Wave");
				person.GetComponent<BodyMecanim>().ResetAnimation();
			}),
			person.GetComponent<BehaviorMecanim>().ST_PlayHandGesture(Val.V(() => "WAVE"), Val.V(() => (long)2000))
		);
	}
	protected Node Dance(GameObject person, int index)
	{
		//Debug.Log("\t      Dance");
		//moods[index] = "dance";
		person.GetComponent<BodyMecanim>().ResetAnimation();

		return new Sequence
		(
			//person.GetComponent<BehaviorMecanim>().Node_BodyAnimation(Val.V(() => "WAVE"), Val.V(() => false)),
			new LeafInvoke(() =>
			{
				//moods[index] = "dance";
				if(index == 0) { mood0 = "dance"; }
				else if (index == 1) { mood1 = "dance"; }
				else if (index == 2) { mood2 = "dance"; }
				person.GetComponent<BodyMecanim>().ResetAnimation();
			}),
			person.GetComponent<BehaviorMecanim>().ST_PlayBodyGesture(Val.V(() => "Breakdance"), Val.V(() => (long)2000))
		);
	}
	protected Node Suprised(GameObject person, int index)
	{
		//Debug.Log("\t      Suprised");
		//moods[index] = "suprised";
		person.GetComponent<BodyMecanim>().ResetAnimation();

		return new Sequence
		(
			//person.GetComponent<BehaviorMecanim>().Node_BodyAnimation(Val.V(() => "WAVE"), Val.V(() => false)),
			new LeafInvoke(() =>
			{
				//moods[index] = "suprised";
				if (index == 0) { mood0 = "suprised"; }
				else if (index == 1) { mood1 = "suprised"; }
				else if (index == 2) { mood2 = "suprised"; }
				person.GetComponent<BodyMecanim>().ResetAnimation();
			}),
			person.GetComponent<BehaviorMecanim>().ST_PlayHandGesture(Val.V(() => "SURPRISED"), Val.V(() => (long)2000))
		);
	}
	protected Node Bored(GameObject person, int index)
	{
		//Debug.Log("\t      Bored");
		//moods[index] = "bored";
		//person.GetComponent<BodyMecanim>().ResetAnimation();

		return new Sequence
		(
			//person.GetComponent<BehaviorMecanim>().Node_BodyAnimation(Val.V(() => "WAVE"), Val.V(() => false)),
			new LeafInvoke(() =>
            {
				//moods[index] = "bored";
				if (index == 0) { mood0 = "bored"; }
				else if (index == 1) { mood1 = "bored"; }
				else if (index == 2) { mood2 = "bored"; }
				person.GetComponent<BodyMecanim>().ResetAnimation();
			}),
			person.GetComponent<BehaviorMecanim>().ST_PlayFaceGesture(Val.V(() => "STAYAWAY"), Val.V(() => (long)2000))
		);
	}

	protected Node React(GameObject person, int index)
	{
		Debug.Log("\t      React");
		//moodList = new List<string>(moods);
		//Val<Vector3> position = Val.V(() => people[moodList.IndexOf("bored")].transform.position);
		//person.GetComponent<BodyMecanim>().ResetAnimation();

		Node printMoods = new Sequence(
			new LeafInvoke(() =>
			{
				Debug.Log(mood0); Debug.Log(mood1); Debug.Log(mood2);
			}));

		Node dance_to_bored = new Sequence(
			new LeafAssert(() => (mood0 == "dance" && index == 0) || (mood1 == "dance" && index == 1) || (mood1 == "dance" && index == 1)),//moodList[index] == "dance"),
			new LeafAssert(() => (mood0 == "bored" || mood1 == "bored" || mood2 == "bored")),//moodList.Contains("bored")),
			/*new LeafInvoke(() =>
			{
				//position = Val.V(() => people[moodList.IndexOf("bored")].transform.position);
			}),*/

			new ChooseOne(
						new Sequence(
							new LeafInvoke(() =>
							{
								person.GetComponent<BodyMecanim>().ResetAnimation();
							}),
							person.GetComponent<BehaviorMecanim>().ST_PlayHandGesture(Val.V(() => "CRY"), Val.V(() => (long)2000))
						),

						new Sequence(
							new LeafInvoke(() =>
							{
								//position = Val.V(() => people[moodList.IndexOf("bored")].transform.position);
								person.GetComponent<BodyMecanim>().ResetAnimation();
							}),
							person.GetComponent<BehaviorMecanim>().Node_BodyAnimation(Val.V(() => "Breakdance"), Val.V(() => false)),
							//person.GetComponent<BehaviorMecanim>().Node_OrientTowards(position),
							//person.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(position, .5f),
							person.GetComponent<BehaviorMecanim>().ST_PlayBodyGesture(Val.V(() => "FIGHT"), Val.V(() => (long)2000))
							//people[moodList.IndexOf("bored")].GetComponent<BehaviorMecanim>().ST_PlayHandGesture(Val.V(() => "SURRENDER"), Val.V(() => (long)2000))
						)
					)
			);

		Node dance_to_suprised = new Sequence(
			new LeafAssert(() => (mood0 == "dance" && index == 0) || (mood1 == "dance" && index == 1) || (mood1 == "dance" && index == 1)),//moodList[index] == "dance"),
			new LeafAssert(() => (mood0 != "bored" && mood1 != "bored" && mood2 != "bored")),//moodList.Contains("bored")),
			/*new LeafInvoke(() =>
			{
				position = Val.V(() => people[moodList.IndexOf("bored")].transform.position);
			}),*/
			new Sequence(
					person.GetComponent<BehaviorMecanim>().ST_PlayHandGesture(Val.V(() => "CLAP"), Val.V(() => (long)2000))
			));

		/*
		if (moods[index] == "dance")
        {
			if (moods.Contains("bored"))
			{
				Val<Vector3> position = Val.V(() => people[moods.IndexOf("bored")].transform.position);

				return new Sequence
				(printMoods,
					new ChooseOne(
						new Sequence(
							new LeafInvoke(() =>
							{
								person.GetComponent<BodyMecanim>().ResetAnimation();
							}),
							person.GetComponent<BehaviorMecanim>().ST_PlayHandGesture(Val.V(() => "CRY"), Val.V(() => (long)2000))
						),

						new Sequence(
							new LeafInvoke(() =>
							{
								position = Val.V(() => people[moods.IndexOf("bored")].transform.position);
								person.GetComponent<BodyMecanim>().ResetAnimation();
							}),
							person.GetComponent<BehaviorMecanim>().Node_BodyAnimation(Val.V(() => "Breakdance"), Val.V(() => false)),
							person.GetComponent<BehaviorMecanim>().Node_OrientTowards(position),
							person.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(position, .5f),
							person.GetComponent<BehaviorMecanim>().ST_PlayBodyGesture(Val.V(() => "FIGHT"), Val.V(() => (long)2000)),
							people[moods.IndexOf("bored")].GetComponent<BehaviorMecanim>().ST_PlayHandGesture(Val.V(() => "SURRENDER"), Val.V(() => (long)2000))
						)
					)
				);
			}
			else
            {
				return new Sequence
				(
					person.GetComponent<BehaviorMecanim>().ST_PlayHandGesture(Val.V(() => "CLAP"), Val.V(() => (long)2000))
				);
			}
		}*/

		return new Sequence(
				printMoods,
				dance_to_bored,
				dance_to_suprised
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
		Node action = new Sequence(new LeafWait(1000000));

		if(i == alreadyInAction) { return action; }

		if (i == 0)
		{
			alreadyInAction = 0;
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
			alreadyInAction = 1;
			Debug.Log("\t Executing ACTION");
			action = new Sequence(
						new SequenceParallel(
							new ChooseOne(
								Dance(people[0], 0), Suprised(people[0], 0), Bored(people[0], 0)
							),
							new ChooseOne(
								Dance(people[1], 1), Suprised(people[1], 1), Bored(people[1], 1)
							),
							new ChooseOne(
								Dance(people[2], 2), Suprised(people[2], 2), Bored(people[2], 2)
							)
						),

						new LeafWait(1000)
					);
		}
		else if (i == 2)
		{
			alreadyInAction = 2;
			Debug.Log("\t Executing REACTION");
			action = new Sequence(
						new SequenceParallel(
							React(people[0], 0),
							React(people[1], 1),
							React(people[2], 2)
						),

						new LeafWait(1000)
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
							//new DecoratorForceStatus(RunStatus.Success, Input()),
							//new DecoratorForceStatus(RunStatus.Success, MaintainArcs()),
							//new DecoratorForceStatus(RunStatus.Success, Story())
							Behavior(0), new LeafWait(1000), Behavior(1), new LeafWait(1000), Behavior(2), new LeafWait(1000), Behavior(3), new LeafWait(1000)
							)
						);
		return roaming;
	}
	protected Node Input()
	{
		Debug.Log("INPUT PHASE");
		return new DecoratorLoop(
			new LeafInvoke(() => {
				if(timer > 30)
                {
					Blackboard.Input.sceneNumber = 3;
					//Debug.Log("\t Scene Number " + 3);
				}
				else if (timer > 20)
                {
					Blackboard.Input.sceneNumber = 2;
					//Debug.Log("\t Scene Number " + 2);
				}
				else if (timer > 10)
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
		return new DecoratorLoop(
			new Selector(
				CheckArc(0),
				CheckArc(1),
				CheckArc(2),
				CheckArc(3),

			new LeafInvoke(() => {
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
		));
	}

	protected Node Story()
	{
		Debug.Log("STORY PHASE");
		return new Sequence(
			new DecoratorLoop(
				new LeafInvoke(() =>
				{
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
