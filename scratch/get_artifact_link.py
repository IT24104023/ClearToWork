import urllib.request
import re

url = 'https://github.com/IT24104023/ClearToWork/actions/runs/37244321302'
req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})
html = urllib.request.urlopen(req).read().decode('utf-8')
artifacts = re.findall(r'href="(/IT24104023/ClearToWork/actions/runs/37244321302/artifacts/\d+)"', html)
print('Artifacts found:', artifacts)
