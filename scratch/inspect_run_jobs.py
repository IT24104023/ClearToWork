import urllib.request
import re

for r in ['37244321302', '37244321284']:
    url = f"https://github.com/IT24104023/ClearToWork/actions/runs/{r}"
    html = urllib.request.urlopen(urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})).read().decode('utf-8')
    title_match = re.search(r'<title>(.*?)</title>', html)
    print(f"Run {r}: Title: {title_match.group(1) if title_match else 'None'}")
    jobs = re.findall(r'href="/IT24104023/ClearToWork/actions/runs/' + r + r'/job/(\d+)"[^>]*>([^<]+)</a>', html)
    print(f"  Jobs: {jobs}")
    artifacts = re.findall(r'href="([^"]*artifacts[^"]*)"', html)
    print(f"  Artifact links: {artifacts}")
