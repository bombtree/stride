using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Stride.Core.Mathematics;

namespace Stride.Navigation
{
    internal class Crowd
    {
#if false
        public NavigationMesh navigationMesh { get; set; }
        public int MaxAgents { get; set; }

        public CrowdAgentSettings crowdAgentSettings { get; set; }

        public NavigationMeshGroup navMeshGroup { get; set; }
        private ProximityGrid proximityGrid;
        private Vector3 extents;
        private ObstacleAvoidanceQuery obstacleAvoidanceQuery;
        private List<ObstacleAvoidanceParams> obstacleAvoidanceParams;
        private const int MAX_OBSTAVOIDANCE_PARAMS = 8;
        private const int MAX_PATH_QUEUE_NODES = 4096;
        private int maxPathResult = 256;
        private List<Navigation.Poly> pathResult;
        private List<Navigation.PathFindQuery> pathQueue;
        private List<CrowdAgentComponent> agentComponents;


        public void Initialize(NavigationMeshGroup group)
        {
            navMeshGroup = group;
            float agentRadius = group.AgentSettings.Radius;
            extents = new Vector3(agentRadius * 2.0f, agentRadius * 1.5f, agentRadius * 2.0f);
            proximityGrid = new ProximityGrid(MaxAgents * 4, agentRadius * 3);

            obstacleAvoidanceQuery = new ObstacleAvoidanceQuery(6, 8);
            // Init obstacle query params.
            obstacleAvoidanceParams = new List<ObstacleAvoidanceParams>(8);
            for (int i = 0; i < MAX_OBSTAVOIDANCE_PARAMS; ++i)
            {
                obstacleAvoidanceParams.Add(ObstacleAvoidanceParams.Default);
            }

            // Allocate temp buffer for merging paths.
            pathResult = new List<Navigation.Poly>(maxPathResult);

            pathQueue = new List<Navigation.PathFindQuery>(MAX_PATH_QUEUE_NODES);

            agentComponents = new List<CrowdAgentComponent>();

            //m_activeAgents = (dtCrowdAgent**)dtAlloc(sizeof(dtCrowdAgent*) * m_maxAgents, DT_ALLOC_PERM);
            //if (!m_activeAgents)
            //    return false;

            //m_agentAnims = (dtCrowdAgentAnimation*)dtAlloc(sizeof(dtCrowdAgentAnimation) * m_maxAgents, DT_ALLOC_PERM);
            //if (!m_agentAnims)
            //    return false;

            //for (int i = 0; i < m_maxAgents; ++i)
            //{
            //    new(&m_agents[i]) dtCrowdAgent();
            //    m_agents[i].active = false;
            //    if (!m_agents[i].corridor.init(m_maxPathResult))
            //        return false;
            //}

            //for (int i = 0; i < m_maxAgents; ++i)
            //{
            //    m_agentAnims[i].active = false;
            //}

            //// The navquery is mostly used for local searches, no need for large node pool.
            //m_navquery = dtAllocNavMeshQuery();
            //if (!m_navquery)
            //    return false;
            //if (dtStatusFailed(m_navquery->init(nav, MAX_COMMON_NODES)))
            //    return false;

            //return true;
        }

        public void AddAgent(CrowdAgentComponent crowdAgent)
        {
            agentComponents.Add(crowdAgent);
        }

        public void Update(double deltaTime)
        {
            //m_velocitySampleCount = 0;
            //const int debugIdx = debug ? debug->idx : -1;

            //dtCrowdAgent** agents = m_activeAgents;
            //int nagents = getActiveAgents(agents, m_maxAgents);

            // Check that all agents still have valid paths.
            CheckPathValidity(deltaTime);

            // Update async move request and path finder.
            updateMoveRequest(dt);

            // Optimize path topology.
            updateTopologyOptimization(agents, nagents, dt);

            // Register agents to proximity grid.
            grid.clear();
            for (int i = 0; i < nagents; ++i)
            {
                //        dtCrowdAgent* ag = agents[i];
                //        const float* p = ag->npos;
                //        const float r = ag->params.radius;
                //        m_grid->addItem((unsigned short)i, p[0] - r, p[2] - r, p[0] + r, p[2] + r);
            }


            // Get nearby navmesh segments and agents to collide with.
            for (int i = 0; i < nagents; ++i)
            {
                //dtCrowdAgent* ag = agents[i];
                //if (ag->state != DT_CROWDAGENT_STATE_WALKING)
                //	continue;

                //// Update the collision boundary after certain distance has been passed or
                //// if it has become invalid.
                //const float updateThr = ag->params.collisionQueryRange*0.25f;
                //if (dtVdist2DSqr(ag->npos, ag->boundary.getCenter()) > dtSqr(updateThr) ||
                //	!ag->boundary.isValid(m_navquery, &m_filters[ag->params.queryFilterType]))
                //{
                //	ag->boundary.update(ag->corridor.getFirstPoly(), ag->npos, ag->params.collisionQueryRange,
                //						m_navquery, &m_filters[ag->params.queryFilterType]);
                //}
                //  // Query neighbour agents
                //  ag->nneis = getNeighbours(ag->npos, ag->params.height, ag->params.collisionQueryRange,
                //                            ag, ag->neis, DT_CROWDAGENT_MAX_NEIGHBOURS,
                //                            agents, nagents, m_grid);
                //for (int j = 0; j<ag->nneis; j++)
                //	ag->neis[j].idx = getAgentIndex(agents[ag->neis[j].idx]);
            }

            // Find next corner to steer to.
            //for (int i = 0; i < nagents; ++i)
            //{
            //    dtCrowdAgent* ag = agents[i];

            //    if (ag->state != DT_CROWDAGENT_STATE_WALKING)
            //        continue;
            //    if (ag->targetState == DT_CROWDAGENT_TARGET_NONE || ag->targetState == DT_CROWDAGENT_TARGET_VELOCITY)
            //        continue;

            //    // Find corners for steering
            //    ag->ncorners = ag->corridor.findCorners(ag->cornerVerts, ag->cornerFlags, ag->cornerPolys,
            //                                            DT_CROWDAGENT_MAX_CORNERS, m_navquery, &m_filters[ag->params.queryFilterType]);

            //    // Check to see if the corner after the next corner is directly visible,
            //    // and short cut to there.
            //    if ((ag->params.updateFlags & DT_CROWD_OPTIMIZE_VIS) && ag->ncorners > 0)
            //		{
            //    const float* target = &ag->cornerVerts[dtMin(1, ag->ncorners - 1) * 3];
            //    ag->corridor.optimizePathVisibility(target, ag->params.pathOptimizationRange, m_navquery, &m_filters[ag->params.queryFilterType]);

            //    // Copy data for debug purposes.
            //    if (debugIdx == i)
            //    {
            //        dtVcopy(debug->optStart, ag->corridor.getPos());
            //        dtVcopy(debug->optEnd, target);
            //    }
            //}

            //        else
            //{
            //    // Copy data for debug purposes.
            //    if (debugIdx == i)
            //    {
            //        dtVset(debug->optStart, 0, 0, 0);
            //        dtVset(debug->optEnd, 0, 0, 0);
            //    }
            //}
            //	}

            //	// Trigger off-mesh connections (depends on corners).
            //	for (int i = 0; i < nagents; ++i)
            //{
            //    dtCrowdAgent* ag = agents[i];

            //    if (ag->state != DT_CROWDAGENT_STATE_WALKING)
            //        continue;
            //    if (ag->targetState == DT_CROWDAGENT_TARGET_NONE || ag->targetState == DT_CROWDAGENT_TARGET_VELOCITY)
            //        continue;

            //    // Check 
            //    const float triggerRadius = ag->params.radius * 2.25f;
            //    if (overOffmeshConnection(ag, triggerRadius))
            //    {
            //        // Prepare to off-mesh connection.
            //        const int idx = (int)(ag - m_agents);
            //        dtCrowdAgentAnimation* anim = &m_agentAnims[idx];

            //        // Adjust the path over the off-mesh connection.
            //        dtPolyRef refs[2];
            //        if (ag->corridor.moveOverOffmeshConnection(ag->cornerPolys[ag->ncorners - 1], refs,
            //                                                   anim->startPos, anim->endPos, m_navquery))
            //        {
            //            dtVcopy(anim->initPos, ag->npos);
            //            anim->polyRef = refs[1];
            //            anim->active = true;
            //            anim->t = 0.0f;
            //            anim->tmax = (dtVdist2D(anim->startPos, anim->endPos) / ag->params.maxSpeed) *0.5f;

            //ag->state = DT_CROWDAGENT_STATE_OFFMESH;
            //ag->ncorners = 0;
            //ag->nneis = 0;
            //continue;
            //			}

            //            else
            //{
            //    // Path validity check will ensure that bad/blocked connections will be replanned.
            //}
            //		}
            //        }
        }

        private void CheckPathValidity(double deltaTime)
        {
            const int CHECK_LOOKAHEAD = 10;
            const float TARGET_REPLAN_DELAY = 1.0f; // seconds
            for(int i = 0; i < agentComponents.Count; ++i)
            {
                var agentComponent = agentComponents[i];
                bool replan = false;
                if (agentComponent.CrowdAgentState == CrowdAgentState.moving)
                {
                        replan = !agentComponent.CheckPath(deltaTime);
                }

            }

            //    ag->targetReplanTime += dt;

            //    bool replan = false;

            //    // First check that the current location is valid.
            //    const int idx = getAgentIndex(ag);
            //    float agentPos[3];
            //    dtPolyRef agentRef = ag->corridor.getFirstPoly();
            //    dtVcopy(agentPos, ag->npos);
            //    if (!m_navquery->isValidPolyRef(agentRef, &m_filters[ag->params.queryFilterType]))
            //    {
            //        // Current location is not valid, try to reposition.
            //        // TODO: this can snap agents, how to handle that?
            //        float nearest[3];
            //        dtVcopy(nearest, agentPos);
            //        agentRef = 0;
            //        m_navquery->findNearestPoly(ag->npos, m_agentPlacementHalfExtents, &m_filters[ag->params.queryFilterType], &agentRef, nearest);
            //        dtVcopy(agentPos, nearest);

            //        if (!agentRef)
            //        {
            //            // Could not find location in navmesh, set state to invalid.
            //            ag->corridor.reset(0, agentPos);
            //            ag->partial = false;
            //            ag->boundary.reset();
            //            ag->state = DT_CROWDAGENT_STATE_INVALID;
            //            continue;
            //        }

            //        // Make sure the first polygon is valid, but leave other valid
            //        // polygons in the path so that replanner can adjust the path better.
            //        ag->corridor.fixPathStart(agentRef, agentPos);
            //        //			ag->corridor.trimInvalidPath(agentRef, agentPos, m_navquery, &m_filter);
            //        ag->boundary.reset();
            //        dtVcopy(ag->npos, agentPos);

            //        replan = true;
            //    }

            //    // If the agent does not have move target or is controlled by velocity, no need to recover the target nor replan.
            //    if (ag->targetState == DT_CROWDAGENT_TARGET_NONE || ag->targetState == DT_CROWDAGENT_TARGET_VELOCITY)
            //        continue;

            //    // Try to recover move request position.
            //    if (ag->targetState != DT_CROWDAGENT_TARGET_NONE && ag->targetState != DT_CROWDAGENT_TARGET_FAILED)
            //    {
            //        if (!m_navquery->isValidPolyRef(ag->targetRef, &m_filters[ag->params.queryFilterType]))
            //        {
            //            // Current target is not valid, try to reposition.
            //            float nearest[3];
            //            dtVcopy(nearest, ag->targetPos);
            //            ag->targetRef = 0;
            //            m_navquery->findNearestPoly(ag->targetPos, m_agentPlacementHalfExtents, &m_filters[ag->params.queryFilterType], &ag->targetRef, nearest);
            //            dtVcopy(ag->targetPos, nearest);
            //            replan = true;
            //        }
            //        if (!ag->targetRef)
            //        {
            //            // Failed to reposition target, fail moverequest.
            //            ag->corridor.reset(agentRef, agentPos);
            //            ag->partial = false;
            //            ag->targetState = DT_CROWDAGENT_TARGET_NONE;
            //        }
            //    }

            //    // If nearby corridor is not valid, replan.
            //    if (!ag->corridor.isValid(CHECK_LOOKAHEAD, m_navquery, &m_filters[ag->params.queryFilterType]))
            //    {
            //        // Fix current path.
            //        //			ag->corridor.trimInvalidPath(agentRef, agentPos, m_navquery, &m_filter);
            //        //			ag->boundary.reset();
            //        replan = true;
            //    }

            //    // If the end of the path is near and it is not the requested location, replan.
            //    if (ag->targetState == DT_CROWDAGENT_TARGET_VALID)
            //    {
            //        if (ag->targetReplanTime > TARGET_REPLAN_DELAY &&
            //            ag->corridor.getPathCount() < CHECK_LOOKAHEAD &&
            //            ag->corridor.getLastPoly() != ag->targetRef)
            //            replan = true;
            //    }

            //    // Try to replan path to goal.
            //    if (replan)
            //    {
            //        if (ag->targetState != DT_CROWDAGENT_TARGET_NONE)
            //        {
            //            requestMoveTargetReplan(idx, ag->targetRef, ag->targetPos);
            //        }
            //    }
            //}
        }
#endif
    }
}

